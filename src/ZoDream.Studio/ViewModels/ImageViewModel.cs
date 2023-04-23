using System.Collections.Generic;
using System.Drawing;
using System.Windows.Input;
using ZoDream.Shared.ViewModel;
using ZoDream.Studio.Controls;
using ZoDream.Studio.Routes;

namespace ZoDream.Studio.ViewModels
{
    public class ImageViewModel: BindableBase, IQueryAttributable
    {

        public ImageViewModel()
        {
            BackCommand = new RelayCommand(TapBack);
            ConfirmCommand = new RelayCommand(TapConfirm);
        }

        public event PreviewUpdatedEventHandler? PreviewUpdated;


        private Bitmap? imageBitmap;

        public Bitmap? ImageBitmap {
            get => imageBitmap;
            set => Set(ref imageBitmap, value);
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

        public void ApplyQueryAttributes(IDictionary<string, object>? queries = null)
        {
            object? arg;
            if (queries is not null && queries.TryGetValue("file", out arg) && arg is string file)
            {
                try
                {
                    ImageBitmap?.Dispose();
                    ImageBitmap = (Bitmap)Image.FromFile(file, true);
                    PreviewUpdated?.Invoke();
                }
                catch (System.Exception)
                {
                    
                }

            }
        }

    }
}
