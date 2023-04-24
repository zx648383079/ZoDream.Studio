using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class AppOption
    {
        public List<HistoryItem> Histories { get; set; } = new();

        public List<AudioSourceFileItem> AudioItems { get; set; } = new();

        public List<PluginItem> PluginItems { get; set; } = new();
        /// <summary>
        /// FFMpeg 所在的文件夹
        /// </summary>
        public string BinFolder { get; set; } = string.Empty;
        /// <summary>
        /// 缓存文件夹
        /// </summary>
        public string TempFolder { get; set; } = string.Empty;
    }
}
