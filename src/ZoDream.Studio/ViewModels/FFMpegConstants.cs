using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Studio.ViewModels
{
    public static class FFMpegConstants
    {
        public static EncoderItem[] VideoEncoderItems = new[]
        {
            new EncoderItem("H.264 (x264)", "libx264"),
            new EncoderItem("H.265 (x265)", "libx265"),
        };

        public static EncoderItem[] AudioEncoderItems = new[]
        {
            new EncoderItem("AAC", "aac"),
        };

        public static int[] AudioSamplingRateItems = new[]
        {
            8000,
            11025,
            16000,
            22050,
            32000,
            44100,
            48000,
            64000,
            88200,
            96000,
            176400,
            192000,
        };
    }
}
