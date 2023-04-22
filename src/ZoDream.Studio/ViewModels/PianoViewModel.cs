using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZoDream.Shared.ViewModel;
using ZoDream.Studio.Pages;
using ZoDream.Studio.Routes;

namespace ZoDream.Studio.ViewModels
{
    public class PianoViewModel: BindableBase
    {

        public PianoViewModel()
        {
            PlayCommand = new RelayCommand(TapPlay);
            StopCommand = new RelayCommand(TapStop);
            BackCommand = new RelayCommand(TapBack);
            ConfirmCommand = new RelayCommand(TapConfirm);
            SettingCommand = new RelayCommand(TapSetting);
            PreviewCommand = new RelayCommand(TapPreview);
        }

        private bool paused = true;

        public bool Paused {
            get => paused;
            set => Set(ref paused, value);
        }

        public ICommand PlayCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand BackCommand { get; private set; }
        public ICommand ConfirmCommand { get; private set; }

        public ICommand PreviewCommand { get; private set; }
        public ICommand SettingCommand { get; private set; }

        private void TapPreview(object? _)
        {
            var page = new PianoWindow();
            page.ShowDialog();
        }

        private void TapSetting(object? _)
        {
            var page = new PianoSettingDialog();
            page.ShowDialog();
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

        }
        private void TapStop(object? _)
        {

        }
    }
}
