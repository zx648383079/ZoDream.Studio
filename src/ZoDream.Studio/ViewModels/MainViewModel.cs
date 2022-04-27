using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Storage;

namespace ZoDream.Studio.ViewModels
{
    public class MainViewModel: BindableBase
    {

        private bool paused = true;

        public bool Paused
        {
            get => paused;
            set => Set(ref paused, value);
        }


        public async Task LoadOptionAsync()
        {
            var res = await AppData.LoadAsync<object>();
            if (res == null)
            {

            }
        }

        public async Task SaveOptionAsync()
        {
            await AppData.SaveAsync(new { });
        }
    }
}
