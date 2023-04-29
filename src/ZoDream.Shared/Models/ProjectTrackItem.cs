using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class ProjectTrackItem
    {
        /// <summary>
        /// 第几行，越小越靠前
        /// </summary>
        public int Index { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        /// <summary>
        /// ms
        /// </summary>
        public long Offset { get; set; }

        public bool IsLocked { get; set; }

        public bool IsHidden { get; set; }

        public TrackType Type { get; set; }

        public ITrackItem? Data { get; set; }

        public static TrackType GetType(ITrackItem? data)
        {
            if (data == null)
            {
                return TrackType.Unknown;
            }
            if (data is VideoTrackItem)
            {
                return TrackType.Video;
            }
            if (data is ImageTrackItem)
            {
                return TrackType.Image;
            }
            if (data is AudioTrackItem)
            {
                return TrackType.Audio;
            }
            if (data is TextTrackItem)
            {
                return TrackType.Text;
            }
            if (data is MidiTrackItem)
            {
                return TrackType.Midi;
            }
            return TrackType.Unknown;
        }
    }
}
