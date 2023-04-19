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
    public class TextViewModel: BindableBase
    {

        public TextViewModel()
        {
            BackCommand = new RelayCommand(TapBack);
            ConfirmCommand = new RelayCommand(TapConfirm);
        }

        public ICommand BackCommand { get; private set; }
        public ICommand ConfirmCommand { get; private set; }

        private void TapBack(object? _)
        {
            ShellManager.GoBackAsync();
        }

        private void TapConfirm(object? _)
        {

        }

    }
}
