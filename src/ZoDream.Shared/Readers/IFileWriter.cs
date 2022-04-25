using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Readers
{
    public interface IFileWriter
    {
        public Task WriteAsync(string file, MusicItem data);
    }
}
