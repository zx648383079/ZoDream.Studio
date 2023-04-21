using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public interface ITrackItem
    {
        public string Name { get; set; }


        public TimeSpan Duration { get; }
    }
}
