using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Studio.ViewModels
{
    public class CreateVoiceViewModel: BindableBase
    {
        public CreateVoiceViewModel()
        {
            StepCommand = new RelayCommand(TapStep);
        }

        public Action<bool>? FinishFn { get; set; }

        private int step;

        public int Step {
            get => step;
            set => Set(ref step, value);
        }

        public ICommand StepCommand { get; private set; }

        private void TapStep(object? arg)
        {
            if (arg is int o)
            {
                Step = o;
            }
            else if (int.TryParse(arg?.ToString(), out var r))
            {
                Step = r;
            }
            else
            {
                Step = 0;
            }
        }
    }
}
