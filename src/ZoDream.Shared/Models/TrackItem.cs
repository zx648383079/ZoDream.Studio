using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    /// <summary>
    /// 一个轨道
    /// </summary>
    public class TrackItem
    {
        public string Color { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public List<RollItem> Items { get; set; } = new();
    }
}
