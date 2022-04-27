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
using ZoDream.Shared.Players;
using ZoDream.Studio.Pages;
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
            DataContext = ViewModel;
        }

        public MainViewModel ViewModel = new();
        private IPlayer? Player;

        private void PianoTb_OnPress(object sender, Controls.PianoKeyEventArgs e)
        {
            Player?.Play(e.Key);
        }

        private void PianoTb_OnRelease(object sender, Controls.PianoKeyEventArgs e)
        {
            Player?.Stop(e.Key);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Player = new MidiPlayer();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            Player?.Dispose();
        }

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Paused = false;
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Paused = true;
        }

        private void OptionBtn_Click(object sender, RoutedEventArgs e)
        {
            var model = new SettingViewModel();
            var page = new SettingWindow(model);
            page.Show();
            model.PropertyChanged += (_, e) =>
            {

            };
        }
    }
}
