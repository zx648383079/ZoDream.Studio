using FFMpegCore;
using FFMpegCore.Extensions.System.Drawing.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using ZoDream.Shared.ViewModel;
using ZoDream.Studio.Routes;
using ZoDream.Studio.Controls;

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

        public Bitmap? ImageBitmap { get; private set; }

        public event PreviewUpdatedEventHandler? PreviewUpdated;

        private bool paused = true;

        public bool Paused {
            get => paused;
            set => Set(ref paused, value);
        }

        public ICommand PlayCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand BackCommand { get; private set; }
        public ICommand ConfirmCommand { get; private set; }

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
            var mediaInfo = await FFProbe.AnalyseAsync(file);
            if (mediaInfo.PrimaryVideoStream is null)
            {
                return;
            }
            ImageBitmap = FFMpegImage.Snapshot(file, new Size(mediaInfo.PrimaryVideoStream.Width, mediaInfo.PrimaryVideoStream.Height), TimeSpan.FromMinutes(1));
            PreviewUpdated?.Invoke();
        }
    }
}
