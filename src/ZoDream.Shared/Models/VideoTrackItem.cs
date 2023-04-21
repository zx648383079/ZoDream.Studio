using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class VideoTrackItem: ITrackItem
    {
        public string FileName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public int BeginFrame { get; set; }

        public int EndFrame { get; set; }

        public TimeSpan Duration { get; set; }
    }
}
