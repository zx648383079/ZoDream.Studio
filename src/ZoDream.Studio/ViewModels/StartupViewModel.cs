using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZoDream.Shared.Models;

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
            HistoryItems.Add(new HistoryItem()
            {
                Name = "Home",
                FileName = "Home",
            });
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
            var picker = new Microsoft.Win32.OpenFileDialog
            {
                Title = "选择项目文件",
                RestoreDirectory = true,
            };
            if (picker.ShowDialog() != true)
            {
                return;
            }
            HistoryItems.Add(new HistoryItem()
            {
                Name = Path.GetFileNameWithoutExtension(picker.FileName),
                FileName = picker.FileName,
            });
        }

        private void TapNew(object? _)
        {

        }

        private void TapHistoryOpen(object? arg) 
        {
            if (arg is HistoryItem item)
            {
                HistoryItems.Remove(item);
            }
        }

        private void TapHistoryDelete(object? arg)
        {

        }
    }
}
