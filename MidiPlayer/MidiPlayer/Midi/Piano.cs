using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace MidiPlayer.Midi
{
    public class Piano
    {
        public Player Driver { get; set; }

        public Canvas Box { get; set; }

        public double Width { get; private set; } = 90;

        public double Height { get; private set; } = 280;

        public IList<PianoKay> KeyList { get; set; } = new List<PianoKay>();

        public Piano(Canvas box)
        {
            Box = box;
            Driver = new Player();
            Driver.Ready();
        }

        public void Refresh()
        {

            if (KeyList.Count < 1)
            {
                CreateKey();
                return;
            }
            foreach (var item in KeyList)
            {
                item.ApplyWithWhite(Width, Height);
            }
        }

        public void RefreshBox(double width, double height)
        {
            RefreshWhite(width / 68, height);
        }

        public void RefreshWhite(double width, double height)
        {
            Width = width;
            Height = height;
            Refresh();
        }

        public PianoKay GetKeyByCode(byte code)
        {
            foreach (var item in KeyList)
            {
                if (item.Code == code)
                {
                    return item;
                }
            }
            return null;
        }

        public async Task TapAsync(byte key, int time)
        {
            var item = GetKeyByCode(key);
            if (item == null)
            {
                return;
            }
            await TapKeyAsync(item, time);
        }

        public async Task TapAsync(Tuple<byte, int> tuple)
        {
            await TapAsync(tuple.Item1, tuple.Item2);
        }

        public async Task PressAsync(byte key, int time)
        {
            var item = GetKeyByCode(key);
            if (item == null)
            {
                return;
            }
            item.IsPress = true;
            await Task.Delay(1000);
            item.IsPress = false;
        }

        public async Task PressAsync(Tuple<byte, int> tuple)
        {
            await PressAsync(tuple.Item1, tuple.Item2);
        }

        public void PressDown(byte key)
        {
            var item = GetKeyByCode(key);
            if (item == null)
            {
                return;
            }
            item.IsPress = true;
        }

        public void PressUp(byte key)
        {
            var item = GetKeyByCode(key);
            if (item == null)
            {
                return;
            }
            item.IsPress = false;
        }

        protected void CreateKey()
        {
            for (int i = 60; i < 72; i++)
            {
                var key = new PianoKay
                {
                    Code = i
                };
                key.ApplyWithWhite(Width, Height, 60);
                Box.Children.Add(key);
                KeyList.Add(key);
                key.Tapped += Key_Tapped;
            }
        }

        private void Key_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            TapKeyAsync(sender as PianoKay);
        }

        public async Task TapKeyAsync(PianoKay key)
        {
            await TapKeyAsync(key, 1000);
        }

        public async Task TapKeyAsync(PianoKay key, int time)
        {
            key.IsPress = true;
            Driver.Play(Convert.ToByte(key.Code));
            await Task.Delay(time);
            Driver.Stop();
            key.IsPress = false;
        }

        public void Dispose()
        {
            KeyList.Clear();
            Driver?.Dispose();
        }
    }
}
