using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class AudioTrackItem: ITrackItem
    {
        public string FileName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public TimeSpan BeginAt { get; set; }

        public TimeSpan EndAt { get; set; }

        public TimeSpan Duration => EndAt - BeginAt;
    }
}
