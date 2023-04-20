using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZoDream.Shared.ViewModel;
using ZoDream.Studio.Routes;

namespace ZoDream.Studio.ViewModels
{
    public class ExportViewModel: BindableBase
    {

        public ExportViewModel()
        {
            StartCommand = new RelayCommand(TapStart);
            StopCommand = new RelayCommand(TapStop);
            BackCommand = new RelayCommand(TapBack);
        }

        private bool paused = true;

        public bool Paused {
            get => paused;
            set => Set(ref paused, value);
        }

        public ICommand StartCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand BackCommand { get; private set; }

        private void TapBack(object? _)
        {
            ShellManager.GoBackAsync();
        }

        private void TapStart(object? _)
        {

        }
        private void TapStop(object? _)
        {

        }
    }
}
