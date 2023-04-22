using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZoDream.Studio.ViewModels;

namespace ZoDream.Studio.Pages
{
    /// <summary>
    /// VideoPage.xaml 的交互逻辑
    /// </summary>
    public partial class VideoPage : Page
    {
        public VideoPage()
        {
            InitializeComponent();
            Loaded += ImagePage_Loaded;
            Unloaded += ImagePage_Unloaded;
        }


        public VideoViewModel ViewModel => (VideoViewModel)DataContext;



        private void ImagePage_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void ImagePage_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.StopCommand.Execute(null);
            ViewModel.ImageBitmap?.Dispose();
        }
    }
}
