using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class VideoTrackItem: IFileTrackItem
    {
        public string FileName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public int BeginFrame { get; set; }

        public int EndFrame { get; set; }
        /// <summary>
        /// 是否是静音
        /// </summary>
        public bool IsMute { get; set; }

        public TimeSpan Duration { get; set; }
    }
}
