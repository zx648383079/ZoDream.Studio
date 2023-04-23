using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ZoDream.Shared.ViewModel;
using ZoDream.Studio.Pages;

namespace ZoDream.Studio.ViewModels
{
    public class PreviewProjectViewModel: BindableBase, IDisposable
    {
        public PreviewProjectViewModel()
        {
            Context = new PreviewProjectDialog()
            {
                DataContext = this
            };
            PlayCommand = new RelayCommand(TapPlay);
            StopCommand = new RelayCommand(TapStop);
            NextCommand = new RelayCommand(TapNext);
            PauseCommand = new RelayCommand(TapPause);
            PreviousCommand = new RelayCommand(TapPrevious);
            Timer.Tick += Timer_Tick;
        }

        private readonly Window Context;
        private readonly DispatcherTimer Timer = new();


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
        public bool AudioVolume {
            get => audioVolume;
            set => Set(ref audioVolume, value);
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
        public ICommand PauseCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand NextCommand { get; private set; }
        public ICommand PreviousCommand { get; private set; }

        private void TapNext(object? _)
        {
            
        }

        private void TapPrevious(object? _)
        {
            
        }

        private void TapPlay(object? _)
        {
            Timer.Interval = TimeSpan.FromMilliseconds(1000);
            Timer.Start();
            Paused = false;
        }
        private void TapPause(object? _)
        {
            Timer.Stop();
            Paused = true;
        }

        private void TapStop(object? _)
        {
            TapPause(null);
            Context.Hide();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            
        }

        public void Show()
        {
            Context.Show();
        }

        public void Close()
        {
            TapStop(null);
        }


        public void Dispose()
        {
            TapStop(null);
            ImageBitmap?.Dispose();
            Context.Close();
        }
    }
}
