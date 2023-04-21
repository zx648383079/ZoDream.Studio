using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class MidiTrackItem: ITrackItem
    {
        public string FileName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public int FrameRate { get; set; } = 130;

        public int FrameCount { get; set; }

        public TimeSpan Duration => TimeSpan.FromSeconds(FrameCount / FrameRate);

        public List<NoteItem> Items { get; set; } = new();
    }
}
