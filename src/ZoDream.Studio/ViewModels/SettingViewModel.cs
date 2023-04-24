using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ZoDream.Shared.Models;
using ZoDream.Shared.Players;
using ZoDream.Shared.ViewModel;
using ZoDream.Studio.Extensions;
using ZoDream.Studio.Pages;
using ZoDream.Studio.Plugin;
using ZoDream.Studio.Routes;

namespace ZoDream.Studio.ViewModels
{
    public class SettingViewModel: BindableBase, IQueryAttributable
    {

        public SettingViewModel()
        {
            BackCommand = new RelayCommand(TapBack);
            ConfirmCommand = new RelayCommand(TapConfirm);
            StepCommand = new RelayCommand(TapStep);
            CreateVoiceCommand = new RelayCommand(TapCreateVoice);
            TempCommand = new RelayCommand(TapTemp);
            BinCommand = new RelayCommand(TapBin);
            PluginToggleCommand = new RelayCommand(TapPluginToggle);
            PluginImportCommand = new RelayCommand(TapPluginImport);
            AudioImportCommand = new RelayCommand(TapAudioImport);
            AudioDeleteCommand = new RelayCommand(TapRemoveAudio);
        }

        private int step;

        public int Step {
            get => step;
            set => Set(ref step, value);
        }

        private string projectName = string.Empty;

        public string ProjectName {
            get => projectName;
            set => Set(ref projectName, value);
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

        private string tempFolder = string.Empty;

        public string TempFolder {
            get => tempFolder;
            set => Set(ref tempFolder, value);
        }

        private string binFolder = string.Empty;

        public string BinFolder {
            get => binFolder;
            set => Set(ref binFolder, value);
        }

        private ObservableCollection<PluginItem> pluginItems = new();

        public ObservableCollection<PluginItem> PluginItems {
            get => pluginItems;
            set => Set(ref pluginItems, value);
        }

        private ObservableCollection<AudioSourceFileItem> audioItems = new();

        public ObservableCollection<AudioSourceFileItem> AudioItems {
            get => audioItems;
            set => Set(ref audioItems, value);
        }


        private ObservableCollection<DataItem> breadCrumbItems = new();

        public ObservableCollection<DataItem> BreadCrumbItems {
            get => breadCrumbItems;
            set => Set(ref breadCrumbItems, value);
        }

        public ICommand BackCommand { get; private set; }
        public ICommand ConfirmCommand { get; private set; }
        public ICommand StepCommand { get; private set; }
        public ICommand CreateVoiceCommand { get; private set; }
        public ICommand TempCommand { get; private set; }
        public ICommand BinCommand { get; private set; }
        public ICommand PluginToggleCommand { get; private set; }
        public ICommand PluginImportCommand { get; private set; }
        public ICommand AudioImportCommand { get; private set; }

        public ICommand AudioDeleteCommand { get; private set; }

        private void TapRemoveAudio(object? arg)
        {
            if (arg is AudioSourceFileItem item)
            {
                AudioItems.Remove(item);
            }
        }

        private void TapAudioImport(object? _)
        {
            var picker = new Microsoft.Win32.OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                RestoreDirectory = true,
                Filter = "音源配置文件|*.json|All|*.*",
                Multiselect = true,
            };
            if (picker.ShowDialog() != true)
            {
                return;
            }
            AudioImport(picker.FileNames);
        }

        private void TapPluginImport(object? _)
        {
            var picker = new Microsoft.Win32.OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                RestoreDirectory = true,
                Filter = "DLL|*.dll|All|*.*",
                Multiselect = true,
            };
            if (picker.ShowDialog() != true)
            {
                return;
            }
            PluginImport(picker.FileNames);
        }

        private void TapPluginToggle(object? arg)
        {
            if (arg is PluginItem item)
            {
                item.IsActive = !item.IsActive;
            }
        }

        private void TapBin(object? _)
        {
            var folder = new System.Windows.Forms.FolderBrowserDialog
            {
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };
            if (folder.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            var bin = Path.Combine(folder.SelectedPath, "ffmpeg.exe");
            if (!File.Exists(bin))
            {
                MessageBox.Show("文件夹不存在 ffmpeg.exe");
                return;
            }
            BinFolder = folder.SelectedPath;
        }

        private void TapTemp(object? _)
        {
            var folder = new System.Windows.Forms.FolderBrowserDialog
            {
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };
            if (folder.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            TempFolder = folder.SelectedPath;
        }

        private void TapCreateVoice(object? _)
        {
            var page = new CreateVoiceDialog();
            page.ShowDialog();
        }

        private void TapBack(object? _)
        {
            if (Step > 0)
            {
                Step = 0;
                AddStep(Step);
                return;
            }
            ShellManager.GoBackAsync();
        }



        private void TapStep(object? arg)
        {
            Step = ConvertToInt(arg);
            AddStep(Step);
        }

        private int ConvertToInt(object? arg)
        {
            if (arg is DataItem item)
            {
                arg = item.Value;
            }
            if (arg is int o)
            {
                return o;
            }
            else if (int.TryParse(arg?.ToString(), out var r))
            {
                return r;
            }
            else
            {
                return 0;
            }
        }

        private void AddStep(int val)
        {
            if (val < 1)
            {
                BreadCrumbItems.Clear();
                return;
            }
            for (var i = 0; i < BreadCrumbItems.Count; i++)
            {
                if (val == ConvertToInt(BreadCrumbItems[i].Value))
                {
                    RemoveEnd(i + 1);
                    return;
                }
            }
            BreadCrumbItems.Add(new DataItem(val switch
            {
                1 => "项目设置",
                2 => "音源管理",
                3 => "插件管理",
                4 => "系统设置",
                9 => "关于",
                _ => string.Empty,
            }, val));
        }

        private void RemoveEnd(int start)
        {
            for (int i = BreadCrumbItems.Count - 1; i >= start; i--)
            {
                BreadCrumbItems.RemoveAt(i);
            }
        }

        public void AudioImport(string[] fileNames)
        {
        }
        public void PluginImport(string[] fileNames)
        {
            var items = PluginLoader.Save(fileNames);
            foreach (var item in items)
            {
                PluginItems.Add(item);
            }
        }

        private void TapConfirm(object? _)
        {
            ProjectItem project = App.ViewModel!.Project!;
            AppOption option = App.ViewModel.Option;
            project.Name = ProjectName;
            project.ScreenHeight = ScreenHeight;
            project.ScreenWidth = ScreenWidth;
            option.TempFolder = TempFolder;
            option.BinFolder = BinFolder;
            option.PluginItems = PluginItems.ToList();
            option.AudioItems = AudioItems.ToList();
            _ = App.ViewModel.SaveAsync();
            ShellManager.GoBackAsync();
        }

        public void ApplyQueryAttributes(IDictionary<string, object>? queries = null)
        {
            ProjectItem project = App.ViewModel!.Project!;
            AppOption option = App.ViewModel.Option;
            ProjectName = project.Name;
            ScreenHeight = project.ScreenHeight;
            ScreenWidth = project.ScreenWidth;
            TempFolder = option.TempFolder;
            BinFolder = option.BinFolder;
            PluginItems = option.PluginItems.ToCollection();
            var items = option.AudioItems.ToCollection();
            if (items.Count >= 0)
            {
                using var player = new WinPlayer();
                foreach (var item in player.ChannelItems)
                {
                    items.Add(new AudioSourceFileItem()
                    {
                        Name = item,
                        IsSystem = true,
                    });
                }
            }
            AudioItems = items;
        }
    }
}
