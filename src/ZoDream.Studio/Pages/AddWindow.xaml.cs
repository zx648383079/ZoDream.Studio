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
    /// AddWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddWindow : Window
    {
        public AddWindow()
        {
            InitializeComponent();
            ViewModel.FinishFn = res => {
                DialogResult = res;
            };
        }

        private AddTrackViewModel ViewModel => (AddTrackViewModel)DataContext;

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

    }
}
