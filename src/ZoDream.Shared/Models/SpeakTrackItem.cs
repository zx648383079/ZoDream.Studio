using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class SpeakTrackItem: ITrackItem
    {
        public string Name { get; set; } = string.Empty;

        public TimeSpan Duration { get; set; }

        public List<TextPromptItem> Items { get; set; } = new();
    }
}
