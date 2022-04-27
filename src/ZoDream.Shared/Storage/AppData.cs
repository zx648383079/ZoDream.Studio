using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZoDream.Shared.Storage
{
    public static class AppData
    {

        public static string DefaultFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "setting.xml");
        public static int MaxLazy = 5;// 5s
        public static Task<T?> LoadAsync<T>()
        {
            return LoadAsync<T>(DefaultFileName);
        }

        public static Task<T?> LoadAsync<T>(string fileName)
        {
            return Task.Factory.StartNew(() =>
            {
                if (!File.Exists(fileName))
                {
                    return default;
                }
                try
                {
                    using var reader = LocationStorage.Reader(fileName);
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    var res = serializer.Deserialize(reader);
                    if (res != null)
                    {
                        return (T)res;
                    }
                    return default;
                }
                catch (Exception)
                {
                    return default;
                }
            });
        }

        private static CancellationTokenSource SaveToken = new();

        public static Task<bool> SaveAsync<T>(string fileName, T data)
        {
            SaveToken.Cancel();
            SaveToken = new CancellationTokenSource();
            return Task.Factory.StartNew(() =>
            {
                if (MaxLazy > 0)
                {
                    Thread.Sleep(MaxLazy * 1000);
                }
                if (SaveToken.IsCancellationRequested)
                {
                    return false;
                }
                try
                {
                    using var writer = LocationStorage.Writer(fileName);
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    serializer.Serialize(writer, data);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }, SaveToken.Token);
        }

        public static Task<bool> SaveAsync<T>(T data)
        {
            return SaveAsync(DefaultFileName, data);
        }
    }
}
