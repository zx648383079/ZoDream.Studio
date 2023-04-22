using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using ZoDream.Shared.Models;
using ZoDream.Shared.ViewModel;
using ZoDream.Studio.Routes;

namespace ZoDream.Studio.ViewModels
{
    public class StartupViewModel: BindableBase
    {
        public StartupViewModel()
        {
            OpenCommand = new RelayCommand(TapOpen);
            NewCommand = new RelayCommand(TapNew);
            HistoryDeleteCommand = new RelayCommand(TapHistoryDelete);
            HistoryOpenCommand = new RelayCommand(TapHistoryOpen);
            if (App.ViewModel != null)
            {
                foreach (var item in App.ViewModel.Option.Histories)
                {
                    HistoryItems.Add(item);
                }
            }
        }


        private ObservableCollection<HistoryItem> historyItems = new();

        public ObservableCollection<HistoryItem> HistoryItems {
            get => historyItems;
            set => Set(ref historyItems, value);
        }

        public ICommand OpenCommand { get; private set; }

        public ICommand NewCommand { get; private set; }

        public ICommand HistoryOpenCommand { get; private set; }

        public ICommand HistoryDeleteCommand { get; private set; }


        private void TapOpen(object? _)
        {
            OpenPicker();
        }

        private void TapNew(object? _)
        {
            EnterNew();
        }

        private void TapHistoryOpen(object? arg) 
        {
            if (arg is HistoryItem item)
            {
                _ = App.ViewModel?.LoadProjectAsync(item.FileName);
            }
        }

        private void TapHistoryDelete(object? arg)
        {
            if (arg is HistoryItem item)
            {
                HistoryItems.Remove(item);
            }
        }

        public static bool OpenPicker()
        {
            var picker = new Microsoft.Win32.OpenFileDialog
            {
                Title = "选择项目文件",
                RestoreDirectory = true,
                Filter = MainViewModel.PickerFilter,
            };
            if (picker.ShowDialog() != true)
            {
                return false;
            }
            _ = App.ViewModel?.LoadProjectAsync(picker.FileName);
            return true;
        }

        public static void EnterNew()
        {
            var project = new ProjectItem();
            if (Screen.PrimaryScreen is not null)
            {
                var bound = Screen.PrimaryScreen!.Bounds;
                project.ScreenWidth = bound.Width;
                project.ScreenHeight = bound.Height;
            }
            App.ViewModel?.Enter(project);
            ShellManager.GoToAsync("workspace", new Dictionary<string, object>
            {
                {"project", project}
            });
        }
    }
}
