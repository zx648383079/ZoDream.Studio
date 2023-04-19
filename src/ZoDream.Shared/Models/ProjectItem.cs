using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class ProjectItem
    {

        public string Name { get; set; } = string.Empty;

        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }

        public List<ProjectTrackItem> TrackItems { get; set; } = new();

        public string OutputFileName { get; set; } = string.Empty;
    }
}
