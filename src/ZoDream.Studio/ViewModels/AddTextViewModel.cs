using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Studio.ViewModels
{
    public class AddTextViewModel: BindableBase
    {

        private string content = string.Empty;

        public string Content {
            get => content;
            set => Set(ref content, value);
        }

        private int volume = 100;

        public int Volume {
            get => volume;
            set => Set(ref volume, value);
        }

        private int duration = 2000;

        public int Duration {
            get => duration;
            set => Set(ref duration, value);
        }

    }
}
