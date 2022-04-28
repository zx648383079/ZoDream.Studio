using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class AudioSource
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        /// <summary>
        /// 声音的开始位置/ms
        /// </summary>
        public uint Position { get; set; }
        /// <summary>
        /// 多长作为一段/ms
        /// </summary>
        public uint Duration { get; set; }
        /// <summary>
        /// 每一段的间隔
        /// </summary>
        public uint Gap { get; set; }

        public uint Loop { get; set; }
        /// <summary>
        /// 文件路径或文件夹名
        /// </summary>
        public string Path { get; set; } = string.Empty;

        public string BeginKey { get; set; } = string.Empty;

        public string Endkey { get; set; } = string.Empty;

        public uint StepKey { get; set; }

        public List<AudioSourceItem> Items { get; set; } = new();
    }

    public class AudioSourceItem
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; } = string.Empty;

        public string BeginKey { get; set; } = string.Empty;

        public string Endkey { get; set; } = string.Empty;

        public uint Position { get; set; }
        public uint End { get; set; }

        public uint Loop { get; set; }
    }
}
