using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class AppOption
    {
        public List<HistoryItem> Histories { get; set; } = new();

        public string FFMpegFolder { get; set; } = string.Empty;

        public string TempFolder { get; set; } = string.Empty;
    }
}
