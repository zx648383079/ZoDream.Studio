using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class ProjectItem: IComparer<ProjectTrackItem>
    {

        public string Name { get; set; } = string.Empty;

        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }

        public List<ProjectTrackItem> TrackItems { get; set; } = new();

        public string OutputFileName { get; set; } = string.Empty;


        public TimeSpan Duration => TimeSpan.FromMilliseconds(GetDurationMilliseconds(TrackItems));


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

        public static double GetDurationMilliseconds(IEnumerable<ProjectTrackItem> data)
        {
            var total = 0d;
            foreach (var item in data)
            {
                var duration = item.Offset + item.Data!.Duration.TotalMilliseconds;
                if (total < duration)
                {
                    total = duration;
                }
            }
            return total;
        }

        public int Compare(ProjectTrackItem x, ProjectTrackItem y)
        {
            return x.Index.CompareTo(y.Index);
        }
    }
}
