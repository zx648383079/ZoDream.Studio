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
        }

        public IMediaAnalysis? MediaInfo { get; private set; }
        private string MediaFile  = string.Empty;


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


        public ICommand PlayCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand BackCommand { get; private set; }
        public ICommand ConfirmCommand { get; private set; }
        public ICommand ProgressCommand { get; private set; }


        private void TapBack(object? _)
        {
            ShellManager.GoBackAsync();
        }

        private void TapConfirm(object? _)
        {

        }

        private void TapPlay(object? _)
        {

        }
        private void TapStop(object? _)
        {

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
            App.Current.Dispatcher.Invoke(() => {
                LoadFrameImage(0);
            });
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
                MediaInfo.FrameToTime(frame));
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
