using FFMpegCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Input;
using ZoDream.Shared.ViewModel;
using ZoDream.Studio.Routes;
using ZoDream.Studio.Controls;
using ZoDream.Studio.Extensions;
using System.Collections.ObjectModel;
using ZoDream.Shared.Models;
using System.Windows.Threading;

namespace ZoDream.Studio.ViewModels
{
    public class VideoViewModel: BindableBase, IQueryAttributable
    {

        public VideoViewModel()
        {
            PlayCommand = new RelayCommand(TapPlay);
            StopCommand = new RelayCommand(TapStop);
            BackCommand = new RelayCommand(TapBack);
            ConfirmCommand = new RelayCommand(TapConfirm);
            PreviewCommand = new RelayCommand(TapPreview);
            AddRangeCommand = new RelayCommand(TapAddRange);
            RemoveRangeCommand = new RelayCommand(TapRemoveRange);
            PreviewRangeCommand = new RelayCommand(TapPreviewRange);
            BeginCommand = new RelayCommand(TapAsBegin);
            EndCommand = new RelayCommand(TapAsEnd);
            NextCommand = new RelayCommand(TapNext);
            PreviousCommand = new RelayCommand(TapPrevious);
            Timer.Tick += Timer_Tick;
        }

        public IMediaAnalysis? MediaInfo { get; private set; }
        private string MediaFile  = string.Empty;
        private readonly DispatcherTimer Timer = new();


        public event PreviewUpdatedEventHandler? PreviewUpdated;

        private bool paused = true;

        public bool Paused {
            get => paused;
            set => Set(ref paused, value);
        }

        private Bitmap? imageBitmap;

        public Bitmap? ImageBitmap {
            get => imageBitmap;
            set => Set(ref imageBitmap, value);
        }

        private int mediaWidth;

        public int MediaWidth {
            get => mediaWidth;
            set => Set(ref mediaWidth, value);
        }

        private int mediaHeight;

        public int MediaHeight {
            get => mediaHeight;
            set => Set(ref mediaHeight, value);
        }


        private bool audioVolume = true;
        /// <summary>
        /// 是否包含音频
        /// </summary>
        public bool AudioVolume {
            get => audioVolume;
            set => Set(ref audioVolume, value);
        }


        private int frameCount;

        public int FrameCount {
            get => frameCount;
            set => Set(ref frameCount, value);
        }

        private int frameCurrent;

        public int FrameCurrent {
            get => frameCurrent;
            set {
                Set(ref frameCurrent, value);
                LoadFrameImage(value);
            }
        }


        private string timeTip = string.Empty;

        public string TimeTip {
            get => timeTip;
            set => Set(ref timeTip, value);
        }


        private TimeSpan duration = TimeSpan.MinValue;

        public TimeSpan Duration {
            get => duration;
            set => Set(ref duration, value);
        }

        private int beginFrame;

        public int BeginFrame {
            get => beginFrame;
            set => Set(ref beginFrame, value);
        }

        private int endFrame;

        public int EndFrame {
            get => endFrame;
            set => Set(ref endFrame, value);
        }

        private bool rangeVisible;

        public bool RangeVisible {
            get => rangeVisible;
            set => Set(ref rangeVisible, value);
        }


        private ObservableCollection<VideoTrackItem> frameItems = new();

        public ObservableCollection<VideoTrackItem> FrameItems {
            get => frameItems;
            set => Set(ref frameItems, value);
        }

        public ICommand PlayCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand BackCommand { get; private set; }
        public ICommand ConfirmCommand { get; private set; }
        public ICommand PreviewCommand { get; private set; }
        public ICommand AddRangeCommand { get; private set; }
        public ICommand RemoveRangeCommand { get; private set; }
        public ICommand PreviewRangeCommand { get; private set; }
        public ICommand BeginCommand { get; private set; }
        public ICommand EndCommand { get; private set; }
        public ICommand NextCommand { get; private set; }
        public ICommand PreviousCommand { get; private set; }

        private void TapNext(object? _)
        {
            FrameCurrent = Math.Min(FrameCurrent + 1, FrameCount);
        }

        private void TapPrevious(object? _)
        {
            FrameCurrent = Math.Max(FrameCurrent - 1, 0);
        }


        private void TapAsBegin(object? _)
        {
            BeginFrame = FrameCurrent;
        }

        private void TapAsEnd(object? _)
        {
            EndFrame = FrameCurrent;
            if (BeginFrame >= EndFrame)
            {
                return;
            }
            TapAddRange(null);
        }

        private void TapPreviewRange(object? arg)
        {
            if (arg is VideoTrackItem o)
            {
                FrameCurrent = BeginFrame = o.BeginFrame;
                EndFrame = o.EndFrame;
                RangeVisible = true;
            }
        }
        private void TapPreview(object? arg)
        {
            if (int.TryParse(arg?.ToString(), out var frame))
            {
                FrameCurrent = frame;
            }
            if (RangeVisible && BeginFrame <= EndFrame)
            {
                RangeVisible = false;
            }
        }

        private void TapAddRange(object? _)
        {
            if (BeginFrame > FrameCount || EndFrame > FrameCount || EndFrame < BeginFrame)
            {
                System.Windows.MessageBox.Show("帧数范围不正确");
                return;
            }
            foreach (var item in FrameItems)
            {   
                if (item.BeginFrame == BeginFrame && item.EndFrame == EndFrame)
                {
                    System.Windows.MessageBox.Show("帧数范围重复");
                    return;
                }
            }
            var duration = MediaInfo!.FrameToTime(EndFrame) - MediaInfo!.FrameToTime(BeginFrame);
            if (duration is null)
            {
                System.Windows.MessageBox.Show("帧数范围不正确");
                return;
            }
            RangeVisible = true;
            FrameItems.Add(new VideoTrackItem
            {
                BeginFrame = BeginFrame,
                EndFrame = EndFrame,
                FileName = MediaFile,
                Name = $"{BeginFrame}-{EndFrame}",
                Duration = (TimeSpan)duration,
            });
        }

        private void TapRemoveRange(object? arg)
        {
            if (arg is VideoTrackItem o)
            {
                FrameItems.Remove(o);
            }
            RangeVisible = false;
        }

        private void TapBack(object? _)
        {
            ShellManager.GoBackAsync();
        }

        private void TapConfirm(object? _)
        {

        }

        private void TapPlay(object? _)
        {
            if (MediaInfo is null)
            {
                return;
            }
            Timer.Interval = TimeSpan.FromMilliseconds(1000);
            Timer.Start();
            Paused = false;
        }
        private void TapStop(object? _)
        {
            Timer.Stop();
            Paused = true;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            FrameCurrent = Math.Min(FrameCount, FrameCurrent +
                (int)Math.Floor(MediaInfo!.PrimaryVideoStream!.FrameRate));
            if (FrameCurrent >= FrameCount)
            {
                TapStop(null);
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> queries)
        {
            object? arg;
            if (queries.TryGetValue("file", out arg) && arg is string file)
            {
                _ = LoadFileAsync(file);
            }
        }

        private async Task LoadFileAsync(string file)
        {
            MediaInfo = await FFProbe.AnalyseAsync(file);
            if (MediaInfo.PrimaryVideoStream is null)
            {
                MediaWidth = 0;
                MediaHeight = 0;
                return;
            }
            MediaFile = file;
            FrameCount = (int)MediaInfo.FrameCount();
            Duration = MediaInfo.Duration;
            MediaWidth = MediaInfo.PrimaryVideoStream.Width;
            MediaHeight = MediaInfo.PrimaryVideoStream.Height;
            FrameCurrent = 0;
        }

        private void LoadFrameImage(int frame)
        {
            if (string.IsNullOrEmpty(MediaFile) || MediaInfo is null || frame > frameCount)
            {
                return;
            }
            ImageBitmap?.Dispose();
            try
            {
                ImageBitmap = FFMpegExtension.Snapshot(MediaFile, MediaInfo,
                                new Size(MediaWidth, MediaHeight), 
                                MediaInfo.FrameToTime(Math.Min(frame, FrameCount - 1)));
                TimeTip = $"{frame}/{FrameCount}";
            }
            catch (Exception)
            {
                ImageBitmap = null;
            }
            PreviewUpdated?.Invoke();
        }
    }
}
