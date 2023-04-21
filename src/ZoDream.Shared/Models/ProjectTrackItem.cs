using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class ProjectTrackItem
    {
        public string Name { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public bool IsLocked { get; set; }

        public bool IsHidden { get; set; }

        public TrackType Type { get; set; }

        public ITrackItem? Data { get; set; }
    }
}
