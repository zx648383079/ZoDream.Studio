using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ZoDream.Shared.Models;
using ZoDream.Shared.Players;
using ZoDream.Shared.Storage;
using ZoDream.Studio.Pages;

namespace ZoDream.Studio.ViewModels
{
    public class MainViewModel: BindableBase
    {

        public MainViewModel()
        {
            PlayCommand = new RelayCommand(TapPlay);
            PauseCommand = new RelayCommand(TapPause);
            SettingCommand = new RelayCommand(TapSetting);
            AddCommand = new RelayCommand(TapAdd);
            Player = new MidiPlayer();
            LoadAsync();
        }

        #region 属性

        public IPlayer<PianoKey>? Player { get; private set; }

        

        private bool playVisible;

        public bool PlayVisible {
            get => playVisible;
            set => Set(ref playVisible, value);
        }

        private bool pauseVisible;

        public bool PauseVisible {
            get => pauseVisible;
            set => Set(ref pauseVisible, value);
        }


        private bool paused = true;

        public bool Paused {
            get => paused;
            set => Set(ref paused, value);
        }
        #endregion

        #region 绑定的方法

        public ICommand PlayCommand { get; private set; }
        public ICommand PauseCommand { get; private set; }
        public ICommand SettingCommand { get; private set; }
        public ICommand AddCommand { get; private set; }

        private void TapPlay(object? _)
        {

        }

        private void TapPause(object? _)
        {

        }

        private void TapSetting(object? _)
        {
            var model = new SettingViewModel();
            var page = new SettingWindow(model);
            page.Show();
            model.PropertyChanged += (_, e) => {

            };
        }

        private void TapAdd(object? _)
        {
            if (true)
            {
                var newPage = new ProjectWindow();
                newPage.ShowDialog();
                return;
            }
            var page = new AddWindow();
            if (page.ShowDialog() != true)
            {
                return;
            }
            switch (page.SelectedIndex)
            {
                case 1:
                    new SpeekWindow().ShowDialog();
                    break;
                case 2:
                    break;
                default:
                    new RollWindow().ShowDialog();
                    break;
            }
        }
        #endregion


        public async void LoadAsync()
        {
            await Player!.ReadyAsync();
        }

        public async Task LoadOptionAsync()
        {
            var res = await AppData.LoadAsync<object>();
            if (res == null)
            {

            }
        }

        public async Task SaveOptionAsync()
        {
            await AppData.SaveAsync(new { });
        }

        ~MainViewModel()
        {
            Player?.Dispose();
        }
    }
}
