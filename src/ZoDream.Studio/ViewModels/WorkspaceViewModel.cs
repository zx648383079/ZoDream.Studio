using FFMpegCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using ZoDream.Shared.Models;
using ZoDream.Shared.Readers;
using ZoDream.Shared.ViewModel;
using ZoDream.Studio.Controls;
using ZoDream.Studio.Extensions;
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
            ItemActionCommand = new RelayCommand(TapItemAction);
            DragCommand = new RelayCommand(TapDragFile);
        }

        #region 属性

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

        public ICommand ItemActionCommand { get; private set; }

        public ICommand DragCommand { get; private set; }

        private void TapDragFile(object? arg)
        {
            if (arg is IEnumerable<string> items)
            {
                foreach (var item in items)
                {
                    _ = AddTrackFileAsync(item);
                    // TODO
                }
            }
        }

        private async Task AddTrackFileAsync(string item)
        {
            if (!File.Exists(item))
            {
                return;
            }
            var ext = Path.GetExtension(item);
            var name = Path.GetFileNameWithoutExtension(item);
            if (FileExtension.IsExtension(ext, FileExtension.VideoFile))
            {
                var mediaInfo = await FFProbe.AnalyseAsync(item);
                if (mediaInfo != null)
                {
                    Add(new VideoTrackItem()
                    {
                        FileName = item,
                        Name = name,
                        BeginFrame = 0,
                        IsMute = mediaInfo.AudioStreams.Count == 0,
                        EndFrame = (int)mediaInfo.FrameCount(),
                        Duration = mediaInfo.Duration,
                    });
                    return;
                }
            }
            if (FileExtension.IsExtension(ext, FileExtension.AudioFile))
            {
                var mediaInfo = await FFProbe.AnalyseAsync(item);
                if (mediaInfo != null)
                {
                    Add(new AudioTrackItem()
                    {
                        FileName = item,
                        Name = name,
                        BeginAt = TimeSpan.Zero,
                        EndAt = mediaInfo.Duration,
                    });
                    return;
                }
            }
            if (FileExtension.IsExtension(ext, FileExtension.ImageFile))
            {
                Add(new ImageTrackItem()
                {
                    FileName = item,
                    Name = name,
                    Duration = TimeSpan.FromSeconds(10)
                });
            }
        }

        private void TapItemAction(object? arg)
        {
            if (arg is TrackActionEventArgs o)
            {
                if (o.IsEdit)
                {
                    switch (o.Data.Type)
                    {
                        case TrackType.Image:
                            ShellManager.GoToAsync("image", new Dictionary<string, object>
                            {
                                {"file", (o.Data.Data as ImageTrackItem)!.FileName}
                            });
                            break;
                        case TrackType.Audio:
                            ShellManager.GoToAsync("audio", new Dictionary<string, object>
                            {
                                {"file", (o.Data.Data as AudioTrackItem)!.FileName}
                            });
                            break;
                        case TrackType.Video:
                            ShellManager.GoToAsync("video", new Dictionary<string, object>
                            {
                                {"file", (o.Data.Data as VideoTrackItem)!.FileName}
                            });
                            break;
                        case TrackType.Text:
                            break;
                        default:
                            break;
                    }
                } else
                {
                    TrackItems.Remove(o.Data);
                }
            }
        }

        private void TapSave(object? _)
        {
            App.ViewModel?.SaveProjectAsync();
        }

        private void TapExport(object? _)
        {
            ShellManager.GoToAsync("export");
        }

        private void TapPlay(object? _)
        {
            App.ViewModel!.PreviewView.Show();
        }

        private void TapPause(object? _)
        {
            App.ViewModel!.PreviewView.Close();
        }

        private void TapSetting(object? _)
        {
            ShellManager.GoToAsync("setting");
        }

        private void TapAdd(object? _)
        {
            if (App.ViewModel?.Project is null)
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

        public void Add(ProjectTrackItem item)
        {
            TrackItems.Add(item);
            App.ViewModel!.Project!.TrackItems.Add(item);
            PlayVisible = Paused;
        }

        public void Add(ITrackItem item)
        {
            Add(new ProjectTrackItem()
            {
                Index = TrackItems.Count,
                Name = item.Name,
                Data = item,
                Type = ProjectTrackItem.GetType(item)
            });
        }

        public void ApplyQueryAttributes(IDictionary<string, object>? queries = null)
        {
            TrackItems.Clear();
            if (App.ViewModel?.Project is not null)
            {
                foreach (var item in App.ViewModel.Project.TrackItems)
                {
                    TrackItems.Add(item);
                }
            }
            PlayVisible = TrackItems.Count > 0;
            Paused = true;
            PauseVisible = false;
        }
    }
}
