using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Studio.ViewModels
{
    public class SpeekViewModel: BindableBase
    {
        private bool paused = true;

        public bool Paused
        {
            get => paused;
            set => Set(ref paused, value);
        }
    }
}
