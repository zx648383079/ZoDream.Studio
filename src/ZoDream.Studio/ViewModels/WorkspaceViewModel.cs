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
using ZoDream.Shared.ViewModel;
using ZoDream.Studio.Pages;
using ZoDream.Studio.Routes;

namespace ZoDream.Studio.ViewModels
{
    public class WorkspaceViewModel: BindableBase, IQueryAttributable
    {

        public WorkspaceViewModel()
        {
            PlayCommand = new RelayCommand(TapPlay);
            PauseCommand = new RelayCommand(TapPause);
            SettingCommand = new RelayCommand(TapSetting);
            AddCommand = new RelayCommand(TapAdd);
            ExportCommand = new RelayCommand(TapExport);
            SaveCommand = new RelayCommand(TapSave);
            Player = new MidiPlayer();
            LoadAsync();
        }

        #region 属性

        public IPlayer<PianoKey>? Player { get; private set; }

        public ProjectItem Project { get; private set; } = new();

        public string ProjectFileName { get; private set; } = string.Empty;

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

        private ObservableCollection<ProjectTrackItem> trackItems = new();

        public ObservableCollection<ProjectTrackItem> TrackItems {
            get => trackItems;
            set => Set(ref trackItems, value);
        }

        #endregion

        #region 绑定的方法

        public ICommand PlayCommand { get; private set; }
        public ICommand PauseCommand { get; private set; }
        public ICommand SettingCommand { get; private set; }
        public ICommand AddCommand { get; private set; }
        public ICommand ExportCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }

        private void TapSave(object? _)
        {

        }

        private void TapExport(object? _)
        {
            ShellManager.GoToAsync("export");
        }

        private void TapPlay(object? _)
        {

        }

        private void TapPause(object? _)
        {

        }

        private void TapSetting(object? _)
        {
            ShellManager.GoToAsync("setting");
            //var model = new SettingViewModel();
            //var page = new SettingWindow(model);
            //page.Show();
            //model.PropertyChanged += (_, e) => {

            //};
        }

        private void TapAdd(object? _)
        {
            if (Project is null)
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
        }
        #endregion


        public async void LoadAsync()
        {
            await Player!.ReadyAsync();
        }


        public void ApplyQueryAttributes(IDictionary<string, object> queries)
        {
            if (queries.TryGetValue("file", out var file) && file is string i)
            {
                ProjectFileName = i;
            }
            if (queries.TryGetValue("project", out var project) && project is ProjectItem o)
            {
                Project = o;
            }
            foreach (var item in Project.TrackItems)
            {
                TrackItems.Add(item);
            }
            App.ViewModel!.Project = Project;
            PlayVisible = true;
            Paused = true;
            PauseVisible = false;
        }

        ~WorkspaceViewModel()
        {
            Player?.Dispose();
        }
    }
}
