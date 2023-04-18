using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class HistoryItem
    {
        public string Name { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public DateTime LastAt { get; set; } = DateTime.MinValue;
    }
}
