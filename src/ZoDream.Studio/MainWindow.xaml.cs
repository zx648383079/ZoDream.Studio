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
using ZoDream.Shared.Models;
using ZoDream.Shared.Players;
using ZoDream.Studio.Pages;
using ZoDream.Studio.Routes;
using ZoDream.Studio.ViewModels;

namespace ZoDream.Studio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            ShellManager.RegisterRoute("startup", typeof(StartupPage));
            ShellManager.RegisterRoute("workspace", typeof(WorkspacePage));
            ShellManager.RegisterRoute("speak", typeof(SpeakPage));
            ShellManager.RegisterRoute("ai/speak", typeof(AiSpeakPage));
            ShellManager.RegisterRoute("audio", typeof(AudioPage));
            ShellManager.RegisterRoute("audio/record", typeof(AudioRecordPage));
            ShellManager.RegisterRoute("screen/record", typeof(ScreenRecordPage));
            ShellManager.RegisterRoute("image", typeof(ImagePage));
            ShellManager.RegisterRoute("piano", typeof(PianoPage));
            ShellManager.RegisterRoute("text", typeof(TextPage));
            ShellManager.RegisterRoute("video", typeof(VideoPage));
            ShellManager.RegisterRoute("setting", typeof(SettingPage));
            ShellManager.RegisterRoute("export", typeof(ExportPage));
        }

        public MainViewModel ViewModel => (MainViewModel)DataContext;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShellManager.BindFrame(InnerFrame);
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            ShellManager.UnBind();
            ViewModel.Dispose();
        }
    }
}
