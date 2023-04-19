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
using System.Windows.Shapes;
using ZoDream.Studio.ViewModels;

namespace ZoDream.Studio.Pages
{
    /// <summary>
    /// SpeakWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SpeakWindow : Window
    {
        public SpeakWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        public SpeakViewModel ViewModel = new();

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Paused = false;
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Paused = true;
        }
    }
}
