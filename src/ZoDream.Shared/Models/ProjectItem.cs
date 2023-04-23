using System;
using System.Collections.Generic;
using System.Linq;
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


        public void Prepend(IEnumerable<ProjectTrackItem> data)
        {
            var items = new List<ProjectTrackItem>();
            var i = 0;
            foreach (var item in data)
            {
                item.Index = i ++;
                items.Add(item);
            }
            foreach (var item in TrackItems)
            {
                item.Index = i++;
                items.Add(item);
            }
            TrackItems = items;
        }
    }
}
