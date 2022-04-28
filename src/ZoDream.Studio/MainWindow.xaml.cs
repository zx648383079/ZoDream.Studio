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


        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Player = new MidiPlayer();
            await Player.ReadyAsync();
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

        private void PianoBtn_Click(object sender, RoutedEventArgs e)
        {
            var page = new PianoWindow();
            page.Show();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var page = new RollWindow();
            page.ShowDialog();
        }
    }
}
