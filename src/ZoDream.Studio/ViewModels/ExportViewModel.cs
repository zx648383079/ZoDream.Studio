using FFMpegCore;
using FFMpegCore.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZoDream.Shared.Models;
using ZoDream.Shared.Readers;
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
            var option = App.ViewModel!.Option;
            if (project == null || project.TrackItems.Count < 1)
            {
                return;
            }
            Task.Factory.StartNew(() => {
                Paused = false;
                TaskProgress = 0;
                var tempFiles = new List<string>();
                var first = project.TrackItems.Order(project).First();
                try
                {
                    var command = FFMpegArguments.FromFileInput(ConvertToTs(first, ref tempFiles));
                    foreach (var item in project.TrackItems)
                    {
                        if (item == first)
                        {
                            continue;
                        }
                        command.AddFileInput(ConvertToTs(item, ref tempFiles));
                    }
                    var audioCodec = AudioCodecItems[AudioCodecIndex].Value;//FFMpeg.GetCodec(AudioCodecItems[AudioCodecIndex].Value);
                    var videoCodec = VideoCodecItems[VideoCodecIndex].Value;//FFMpeg.GetCodec(VideoCodecItems[VideoCodecIndex].Value);
                    var audioQuality = Enum.Parse<AudioQuality>(AudioQualityItems[AudioQualityIndex]);
                    var processor = command.OutputToFile(FileName, overwrite: true, delegate (FFMpegArgumentOptions options)
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
                    }, project.Duration);
                    processor.ProcessSynchronously();
                }
                finally
                {
                    CleanUp(tempFiles);
                }
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

        private string ConvertToTs(ProjectTrackItem trackItem, ref List<string> tempFiles)
        {
            if (trackItem.Data is IFileTrackItem o)
            {
                return o.FileName;
            }
            // tempFiles.Add();
            return string.Empty;
        }

        public void ApplyQueryAttributes(IDictionary<string, object>? queries = null)
        {
            var project = App.ViewModel?.Project;
            if (project is null)
            {
                return;
            }
            FileName = project.OutputFileName;
            if (string.IsNullOrWhiteSpace(FileName))
            {
                // FileName = $"{project.Name}.{FormatItems[FormatIndex].ToLower()}";
            }
            ScreenHeight = project.ScreenHeight;
            ScreenWidth = project.ScreenWidth;
        }
    }
}
