using FFMpegCore.Pipes;
using FFMpegCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Studio.Extensions
{
    public static class FFMpegExtension
    {
        public static Bitmap Snapshot(string input, IMediaAnalysis source, Size? size = null, TimeSpan? captureTime = null, int? streamIndex = null, int inputFileIndex = 0)
        {
            FFMpegArguments fFMpegArguments;
            Action<FFMpegArgumentOptions> outputOptions;
            (fFMpegArguments, outputOptions) = SnapshotArgumentBuilder.BuildSnapshotArguments(input, source, size, captureTime, streamIndex, inputFileIndex);
            using var memoryStream = new MemoryStream();
            fFMpegArguments.OutputToPipe(new StreamPipeSink(memoryStream), delegate (FFMpegArgumentOptions options)
            {
                outputOptions(options.ForceFormat("rawvideo"));
            }).ProcessSynchronously();
            memoryStream.Position = 0L;
            using var bitmap = new Bitmap(memoryStream);
            return bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), bitmap.PixelFormat);
        }

        public static double FrameCount(this IMediaAnalysis source)
        {
            var total = 0d;
            foreach (var item in source.VideoStreams)
            {
                total += item.FrameRate * item.Duration.TotalSeconds;
            }
            return total;
        }

        public static TimeSpan? FrameToTime(this IMediaAnalysis source, double frame)
        {
            var total = 0d;
            foreach (var item in source.VideoStreams)
            {
                var min = total;
                total += item.FrameRate * item.Duration.TotalSeconds;
                if (total < frame)
                {
                    continue;
                }
                return item.StartTime + TimeSpan.FromSeconds((frame - min) / item.FrameRate);
            }
            return null;
        }

        public static double FrameRate(this IMediaAnalysis source)
        {
            if (source.PrimaryVideoStream is not null)
            {
                return source.PrimaryVideoStream.FrameRate;
            }
            foreach (var item in source.VideoStreams)
            {
                return item.FrameRate;
            }
            return 0;
        }
    }
}
