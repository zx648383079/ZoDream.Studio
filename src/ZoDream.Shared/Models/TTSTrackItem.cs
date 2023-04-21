using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class TTSTrackItem: ITrackItem
    {
        public string FileName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public TimeSpan Duration { get; set; }

        public string Language { get; set; } = string.Empty;

        public string VoiceTone { get; set; } = string.Empty;

        public int Volume { get; set; }

        public int SpeakSpeed { get; set; }
    }
}
