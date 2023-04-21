﻿using Newtonsoft.Json;
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

        public static string DefaultFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "setting.json");
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
                    return LoadJsonAsync<T>(fileName).GetAwaiter().GetResult();
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
                    SaveJsonAsync(fileName, data).GetAwaiter().GetResult();
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

        private static async Task SaveXmlAsync<T>(string file, T data)
        {
            await Task.Factory.StartNew(() =>
            {
                using var writer = LocationStorage.Writer(file);
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                serializer.Serialize(writer, data);
            });
        }

        private static async Task<T?> LoadXmlAsync<T>(string file)
        {
            return await Task.Factory.StartNew(() =>
            {
                using var reader = LocationStorage.Reader(file);
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                var res = serializer.Deserialize(reader);
                if (res != null)
                {
                    return (T)res;
                }
                return default;
            });
        }

        private static async Task SaveJsonAsync<T>(string file, T data)
        {
            await LocationStorage.WriteAsync(file, JsonConvert.SerializeObject(data, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
            }));
        }

        private static async Task<T?> LoadJsonAsync<T>(string file)
        {
            var str = await LocationStorage.ReadAsync(file);
            if (string.IsNullOrWhiteSpace(str))
            {
                return default;
            }
            return JsonConvert.DeserializeObject<T>(str, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
            });
        }
    }
}
