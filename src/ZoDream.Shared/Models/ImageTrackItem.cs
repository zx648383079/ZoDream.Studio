using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class ImageTrackItem: IFileTrackItem
    {
        public string FileName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public TimeSpan Duration { get; set; }
    }
}
