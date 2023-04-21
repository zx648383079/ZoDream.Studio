using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class TextTrackItem: ITrackItem
    {
        public string Content { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public TimeSpan Duration { get; set; }
    }
}
