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
        }

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
    }
}
