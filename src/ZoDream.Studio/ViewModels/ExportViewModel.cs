using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Studio.ViewModels
{
    public class ExportViewModel: BindableBase
    {

        public ExportViewModel()
        {
            StartCommand = new RelayCommand(TapStart);
            StopCommand = new RelayCommand(TapStop);
        }

        private bool paused = true;

        public bool Paused {
            get => paused;
            set => Set(ref paused, value);
        }

        public ICommand StartCommand { get; private set; }
        public ICommand StopCommand { get; private set; }

        private void TapStart(object? _)
        {

        }
        private void TapStop(object? _)
        {

        }
    }
}
