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
    /// ProjectWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ProjectWindow : Window
    {
        public ProjectWindow()
        {
            InitializeComponent();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NewBtn_Click(object sender, RoutedEventArgs e)
        {
            StartupViewModel.EnterNew();
            Close();
        }

        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            if (StartupViewModel.OpenPicker())
            {
                Close();
            }
        }
    }
}
