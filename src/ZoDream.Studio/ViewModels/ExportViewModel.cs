using FFMpegCore;
using FFMpegCore.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ZoDream.Shared.Models;
using ZoDream.Shared.ViewModel;
using ZoDream.Studio.Routes;
using FileExtension = ZoDream.Shared.Readers.FileExtension;

namespace ZoDream.Studio.ViewModels
{
    public class ExportViewModel: BindableBase, IQueryAttributable
    {

        public ExportViewModel()
        {
            StartCommand = new RelayCommand(TapStart);
            StopCommand = new RelayCommand(TapStop);
            BackCommand = new RelayCommand(TapBack);
            FileFilter = $"视频文件|{FileExtension.FormatFilter(FileExtension.VideoFile)}|音频文件|{FileExtension.FormatFilter(FileExtension.AudioFile)}|所有文件|*.*";
        }

        private bool paused = true;

        public bool Paused {
            get => paused;
            set => Set(ref paused, value);
        }

        private double taskProgress;

        public double TaskProgress {
            get => taskProgress;
            set => Set(ref taskProgress, value);
        }


        private string fileFilter = string.Empty;

        public string FileFilter {
            get => fileFilter;
            set => Set(ref fileFilter, value);
        }


        private string[] formatItems = new[] { "MP3", "FLAC", "APE", "GIF", "MP4", "MKV", "WEBM"};

        public string[] FormatItems {
            get => formatItems;
            set => Set(ref formatItems, value);
        }

        private int formatIndex = 5;

        public int FormatIndex {
            get => formatIndex;
            set => Set(ref formatIndex, value);
        }

        private EncoderItem[] videoCodecItems = FFMpegConstants.VideoEncoderItems;

        public EncoderItem[] VideoCodecItems {
            get => videoCodecItems;
            set => Set(ref videoCodecItems, value);
        }

        private int videoCodecIndex;

        public int VideoCodecIndex {
            get => videoCodecIndex;
            set => Set(ref videoCodecIndex, value);
        }


        private EncoderItem[] audioCodecItems = FFMpegConstants.AudioEncoderItems;

        public EncoderItem[] AudioCodecItems {
            get => audioCodecItems;
            set => Set(ref audioCodecItems, value);
        }


        private int audioCodecIndex;

        public int AudioCodecIndex {
            get => audioCodecIndex;
            set => Set(ref audioCodecIndex, value);
        }

        private string fileName = string.Empty;

        public string FileName {
            get => fileName;
            set => Set(ref fileName, value);
        }

        private bool isAutoDuration = true;

        public bool IsAutoDuration {
            get => isAutoDuration;
            set {
                Set(ref isAutoDuration, value);
                OnPropertyChanged(nameof(DurationText));
            }
        }

        private TimeSpan duration;

        public TimeSpan Duration {
            get => duration;
            set {
                Set(ref duration, value);
                OnPropertyChanged(nameof(DurationText));
            }
        }

        public string DurationText => IsAutoDuration ? "自动" : Duration.ToString();


        private bool isMute;

        public bool IsMute {
            get => isMute;
            set => Set(ref isMute, value);
        }

        private bool isFastest = true;

        public bool IsFastest {
            get => isFastest;
            set => Set(ref isFastest, value);
        }


        private int videoFrameRate = 30;

        public int VideoFrameRate {
            get => videoFrameRate;
            set => Set(ref videoFrameRate, value);
        }


        private int screenWidth;

        public int ScreenWidth {
            get => screenWidth;
            set {
                Set(ref screenWidth, value);
                OnPropertyChanged(nameof(ScreenSize));
            }
        }

        private int screenHeight;

        public int ScreenHeight {
            get => screenHeight;
            set {
                Set(ref screenHeight, value);
                OnPropertyChanged(nameof(ScreenSize));
            }
        }

        public string ScreenSize => $"{screenWidth}x{screenHeight}";

        private int videoQuality = 22;
        /// <summary>
        /// 越小质量越好
        /// </summary>
        public int VideoQuality {
            get => videoQuality;
            set => Set(ref videoQuality, value);
        }

        private string[] audioQualityItems = Enum.GetNames(typeof(AudioQuality));

        public string[] AudioQualityItems {
            get => audioQualityItems;
            set => Set(ref audioQualityItems, value);
        }


        private int audioQualityIndex = 3;

        public int AudioQualityIndex {
            get => audioQualityIndex;
            set => Set(ref audioQualityIndex, value);
        }

        private int[] audioSamplingRateItems = FFMpegConstants.AudioSamplingRateItems;

        public int[] AudioSamplingRateItems {
            get => audioSamplingRateItems;
            set => Set(ref audioSamplingRateItems, value);
        }


        private int audioSamplingRate = 48000;

        public int AudioSamplingRate {
            get => audioSamplingRate;
            set => Set(ref audioSamplingRate, value);
        }

        public ICommand StartCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand BackCommand { get; private set; }

        private Action? CancelFn;

        private void TapBack(object? _)
        {
            ShellManager.GoBackAsync();
        }

        private void TapStart(object? _)
        {
            var project = App.ViewModel?.Project;
            string tempFolder = App.ViewModel!.TempFolder;
            if (project == null || project.TrackItems.Count < 1)
            {
                return;
            }
            Task.Factory.StartNew(() => {
                Paused = false;
                TaskProgress = 0;
                StartExport(project, IsAutoDuration ? project.Duration : Duration, tempFolder);
                TaskProgress = 100;
                Paused = true;
            });
        }
        private void TapStop(object? _)
        {
            CancelFn?.Invoke();
            TaskProgress = 100;
            Paused = true;
        }

        private void CleanUp(IEnumerable<string> tempFiles)
        {
            foreach (var tempFile in tempFiles)
            {
                File.Delete(tempFile);
            }
        }

        private void StartExport(ProjectItem project, TimeSpan maxDuration, string tempFolder)
        {
            var tempFile = ExportVideo(project.TrackItems, maxDuration, tempFolder);
            if (Paused)
            {
                return;
            }
            if (!IsMute)
            {
                tempFile = ExportAudio(project.TrackItems, tempFile, maxDuration, tempFolder);
            }
            
            if (string.IsNullOrEmpty(tempFile))
            {
                return;
            }
            if (tempFile.StartsWith(tempFolder))
            {
                File.Move(tempFile, FileName);
                return;
            }
            File.Copy(tempFile, FileName, true);
        }

        private string ExportVideo(List<ProjectTrackItem> items, TimeSpan maxDuration, string tempFolder)
        {
            var filterItems = items.Where(item => {
                return item.Type switch
                {
                    TrackType.Video or TrackType.Image => true,
                    _ => false
                };
            });
            return ExportConcatFile(filterItems, maxDuration, tempFolder);
        }

        private string ExportConcatFile(IEnumerable<ProjectTrackItem> items, TimeSpan maxDuration, string tempFolder)
        {
            var start = 0d;
            var end = maxDuration.TotalNanoseconds;
            var index = -1;
            var max = end;
            var fileItems = new List<string>();
            var tempFiles = new List<string>();
            while (end >= max)
            {
                ProjectTrackItem? current = null;
                index = -1;
                foreach (var item in items)
                {
                    var itemEnd = item.Offset + item.Data!.Duration.TotalMilliseconds;
                    if (item.Offset <= start && itemEnd > start && (index < 0 || index > item.Index))
                    {
                        start = Math.Max(item.Offset, start);
                        end = Math.Min(start + item.Data!.Duration.TotalMilliseconds, max);
                        index = item.Index;
                        current = item;
                        continue;
                    }
                    if (item.Offset > start && item.Offset < end && (index > item.Index))
                    {
                        end = item.Offset;
                    }
                }
                if (current != null)
                {
                    fileItems.Add(ConvertToTs(current, start, end, tempFolder, ref tempFiles));
                    continue;
                }
                fileItems.Add(ConvertBgToTs(end - start, tempFolder, ref tempFiles));
            }
            try
            {
                var command = FFMpegArguments.FromConcatInput(fileItems);
                var audioCodec = AudioCodecItems[AudioCodecIndex].Value;//FFMpeg.GetCodec(AudioCodecItems[AudioCodecIndex].Value);
                var videoCodec = VideoCodecItems[VideoCodecIndex].Value;//FFMpeg.GetCodec(VideoCodecItems[VideoCodecIndex].Value);
                var audioQuality = Enum.Parse<AudioQuality>(AudioQualityItems[AudioQualityIndex]);
                var processor = command!.OutputToFile(FileName, overwrite: true, delegate (FFMpegArgumentOptions options)
                {
                    options.CopyChannel().WithAudioCodec(audioCodec)
                        .WithAudioBitrate(audioQuality)
                        .WithVideoBitrate(videoFrameRate)
                        .WithVideoCodec(videoCodec)
                        .Resize(ScreenWidth, ScreenHeight)
                        .WithAudioSamplingRate(AudioSamplingRate)
                        .WithConstantRateFactor(VideoQuality)
                        .UsingShortest(!IsFastest);
                });
                processor.CancellableThrough(out CancelFn);
                processor.NotifyOnProgress(progress => {
                    TaskProgress = progress;
                }, maxDuration);
                processor.ProcessSynchronously();
            }
            finally
            {
                CleanUp(tempFiles);
            }
            return string.Empty;
        }

        private string ExportAudio(List<ProjectTrackItem> items, string videoFileName, 
            TimeSpan maxDuration, string tempFolder)
        {
            var filterItems = items.Where(item => {
                return item.Type switch
                {
                    TrackType.Midi or TrackType.Audio => true,
                    _ => false
                };
            });
            if (string.IsNullOrEmpty(videoFileName))
            {
                return ExportConcatFile(filterItems, maxDuration, tempFolder);
            }
            var tempFiles = new List<string>();
            try
            {
                var command = FFMpegArguments.FromFileInput(videoFileName);
                foreach (var item in filterItems)
                {
                    if (Paused)
                    {
                        CleanUp(tempFiles);
                        return string.Empty;
                    }
                    var tempFile = ConvertToTs(item, tempFolder, ref tempFiles);
                    command.AddFileInput(tempFile, false, args => {
                        var start = TimeSpan.FromMilliseconds(item.Offset);
                        args.Seek(start);
                        args.EndSeek(TimeSpan.FromMilliseconds(Math.Min(item.Offset +
                            item.Data!.Duration.TotalMilliseconds, maxDuration.TotalMilliseconds)));
                    });
                }
                var audioCodec = AudioCodecItems[AudioCodecIndex].Value;//FFMpeg.GetCodec(AudioCodecItems[AudioCodecIndex].Value);
                var videoCodec = VideoCodecItems[VideoCodecIndex].Value;//FFMpeg.GetCodec(VideoCodecItems[VideoCodecIndex].Value);
                var audioQuality = Enum.Parse<AudioQuality>(AudioQualityItems[AudioQualityIndex]);
                var processor = command!.OutputToFile(FileName, overwrite: true, delegate (FFMpegArgumentOptions options)
                {
                    options.CopyChannel().WithAudioCodec(audioCodec)
                        .WithAudioBitrate(audioQuality)
                        .WithVideoBitrate(videoFrameRate)
                        .WithVideoCodec(videoCodec)
                        .Resize(ScreenWidth, ScreenHeight)
                        .WithAudioSamplingRate(AudioSamplingRate)
                        .WithConstantRateFactor(VideoQuality)
                        .UsingShortest(!IsFastest);
                });
                processor.CancellableThrough(out CancelFn);
                processor.NotifyOnProgress(progress => {
                    TaskProgress = progress;
                }, maxDuration);
                processor.ProcessSynchronously();
            }
            finally
            {
                CleanUp(tempFiles);
            }
            return string.Empty;
        }

        private string ConvertToTs(ProjectTrackItem trackItem, string tempFolder, ref List<string> tempFiles)
        {
            if (trackItem.Data is IFileTrackItem o)
            {
                return o.FileName;
            }
            // tempFiles.Add();
            return string.Empty;
        }

        private string ConvertToTs(ProjectTrackItem trackItem, double start, double end, string tempFolder, ref List<string> tempFiles)
        {
            string tempFile;
            if (trackItem.Data is IFileTrackItem o)
            {
                tempFile = o.FileName;
            } else
            {
                // 生成文件
                // tempFiles.Add();
                tempFile = string.Empty;
            }
            if (trackItem.Type == TrackType.Image)
            {
                tempFile = Path.Combine(tempFolder, $"temp_{start}-{end}.mp4");
                tempFiles.Add(tempFile);
                FFMpeg.JoinImageSequence(tempFile, end- start, ((ImageTrackItem)trackItem.Data).FileName);
                return tempFile;
            }
            if (trackItem.Offset == start)
            {
                var info = FFProbe.Analyse(tempFile);
                if (info.Duration.TotalMilliseconds == end - start)
                {
                    return tempFile;
                }
            }
            var subTempFile = Path.Combine(tempFolder, $"temp_{start}-{end}.mp4");
            var i = start - trackItem.Offset;
            if (trackItem.Data is VideoTrackItem v)
            {
                i += v.BeginAt;
            } else if (trackItem.Data is AudioTrackItem a)
            {
                i += a.BeginAt.TotalMilliseconds;
            }
            FFMpeg.SubVideo(tempFile, subTempFile, 
                TimeSpan.FromMilliseconds(i),
                TimeSpan.FromMilliseconds(i + end - start));
            tempFiles.Add(subTempFile);
            return subTempFile;
        }

        private string ConvertBgToTs(double duration, string tempFolder, ref List<string> tempFiles)
        {
            var tempFile = Path.Combine(tempFolder, $"empty_{duration}.mp4");
            if (File.Exists(tempFile))
            {
                return tempFile;
            }
            var tempImgFile = Path.Combine(tempFolder, $"empty_bg.bmp");
            tempFiles.Add(tempFile);
            if (!File.Exists(tempImgFile))
            {
                tempFiles.Add(tempImgFile);
                // 转换空白背景
                var bitmap = new Bitmap(ScreenWidth, ScreenHeight);
                bitmap.Save(tempImgFile, ImageFormat.Bmp);
            }
            FFMpeg.JoinImageSequence(tempFile, duration, tempImgFile);
            return tempFile;
        }

        public void ApplyQueryAttributes(IDictionary<string, object>? queries = null)
        {
            var project = App.ViewModel?.Project;
            if (project is null)
            {
                return;
            }
            FileName = project.OutputFileName;
            Duration = project.Duration;
            if (string.IsNullOrWhiteSpace(FileName))
            {
                // FileName = $"{project.Name}.{FormatItems[FormatIndex].ToLower()}";
            }
            ScreenHeight = project.ScreenHeight;
            ScreenWidth = project.ScreenWidth;

        }
    }
}
