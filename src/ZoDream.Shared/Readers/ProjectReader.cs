using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using ZoDream.Shared.Models;
using ZoDream.Shared.Storage;

namespace ZoDream.Shared.Readers
{
    public class ProjectReader
    {
        public async Task<ProjectItem?> ReadAsync(string fileName)
        {
            return await AppData.LoadAsync<ProjectItem>(fileName);
        }

        public async Task WriteAsync(string fileName, ProjectItem data)
        {
            await AppData.SaveAsync(fileName, data);
        }
    }
}
