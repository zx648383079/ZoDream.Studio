using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Studio.ViewModels
{
    public class RollViewModel: BindableBase
    {
        private bool paused = true;

        public bool Paused
        {
            get => paused;
            set => Set(ref paused, value);
        }
    }
}
