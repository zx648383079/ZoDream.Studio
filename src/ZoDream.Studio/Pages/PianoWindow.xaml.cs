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
using ZoDream.Shared.Players;

namespace ZoDream.Studio.Pages
{
    /// <summary>
    /// PianoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PianoWindow : Window
    {
        public PianoWindow()
        {
            InitializeComponent();
        }

        private IPlayer? Player;
        private byte PlayerChanel  = 0;

        private void PianoTb_OnPress(object sender, Controls.PianoKeyEventArgs e)
        {
            Player?.Play(PlayerChanel, e.Key);
        }

        private void PianoTb_OnRelease(object sender, Controls.PianoKeyEventArgs e)
        {
            Player?.Stop(PlayerChanel, e.Key);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Player = new WinPlayer();
            await Player.ReadyAsync();
            TypeTb.ItemsSource = Player.ChannelItems;
            PianoViewer.Start();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            Player?.Dispose();
        }

        private void TypeTb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PlayerChanel = Convert.ToByte(TypeTb.SelectedIndex);
        }
    }
}
