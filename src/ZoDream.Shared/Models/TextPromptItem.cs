using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class TextPromptItem
    {
        public string Content { get; set; } = string.Empty;

        public int Volume { get; set; }

        public int Offset { get; set; }

        public int Duration { get; set; }
    }
}
