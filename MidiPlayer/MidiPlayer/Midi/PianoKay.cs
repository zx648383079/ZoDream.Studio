using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace MidiPlayer.Midi
{
    public class PianoKay: Control
    {
        public PianoKay()
        {
            this.DefaultStyleKey = typeof(PianoKay);
        }

        public int Code
        {
            get { return (int)GetValue(CodeProperty); }
            set { SetValue(CodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Code.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CodeProperty =
            DependencyProperty.Register("Code", typeof(int), typeof(PianoKay), new PropertyMetadata(0));




        public bool IsPress
        {
            get { return (bool)GetValue(IsPressProperty); }
            set {
                SetValue(IsPressProperty, value);
                applyBackground();
            }
        }

        // Using a DependencyProperty as the backing store for IsPress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPressProperty =
            DependencyProperty.Register("IsPress", typeof(bool), typeof(PianoKay), new PropertyMetadata(false));


        private void applyBackground()
        {
            if (IsPress)
            {
                Background = new SolidColorBrush(Colors.Red);
                return;
            }
            Background = new SolidColorBrush(ToKey().Scale > 0 ? Colors.Black : Colors.White);
        }

        public Key ToKey()
        {
            return Key.Create127(Code);
        }
        /// <summary>
        /// 应用属性
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void ApplyWithWhite(double width, double height)
        {
            ApplyWithWhite(width, height, 0);
        }

        /// <summary>
        /// 应用属性
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="offset">偏移数量</param>
        public void ApplyWithWhite(double width, double height, int offset)
        {
            var key = ToKey();
            var x = width * (key.GetWhiteCount() - Key.Create127(offset).GetWhiteCount());
            BorderBrush = new SolidColorBrush(Colors.Black);
            BorderThickness = new Thickness(1, 0, 1, 1);
            if (key.Scale > 0)
            {
                width = width * 2 / 3;
                x += width;
                height = height * 3 / 5;
                Background = new SolidColorBrush(Colors.Black);
                SetValue(Canvas.ZIndexProperty, 69);
            } else
            {
                Background = new SolidColorBrush(Colors.White);
            }
            SetValue(Canvas.TopProperty, 0);
            SetValue(Canvas.LeftProperty, x);
            Width = width;
            Height = height;
        }
    }
}
