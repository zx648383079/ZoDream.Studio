using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Threading;

namespace ZoDream.Studio.Controls
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ZoDream.Studio.Controls"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ZoDream.Studio.Controls;assembly=ZoDream.Studio.Controls"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:PianoScrollViewer/>
    ///
    /// </summary>
    public class PianoScrollViewer : Control
    {
        static PianoScrollViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PianoScrollViewer), new FrameworkPropertyMetadata(typeof(PianoScrollViewer)));
        }

        public PianoScrollViewer()
        {
            Unloaded += PianoScrollViewer_Unloaded;
        }

        private void PianoScrollViewer_Unloaded(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        public Brush NoteBackground
        {
            get { return (Brush)GetValue(NoteBackgroundProperty); }
            set { SetValue(NoteBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NoteBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NoteBackgroundProperty =
            DependencyProperty.Register("NoteBackground", typeof(Brush), typeof(PianoScrollViewer), new PropertyMetadata(new SolidColorBrush(Colors.Green)));



        // private DispatcherTimer? DispatcherTimer;
        private double TempY = 0;

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            var pen = new Pen(NoteBackground, 1);
            var font = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
            var maxHeight = ActualHeight;
            var maxWidth = ActualWidth;
            for (int i = 0; i < 1; i++)
            {
                var x = 0;
                var y = TempY;
                var w = 30;
                var h = 100;
                if (y > maxHeight || x  > maxWidth || x + w < 0)
                {
                    continue;
                }
                drawingContext.DrawRectangle(NoteBackground, pen, new Rect(x, y, w, h));
                var ft = new FormattedText(
                            "C0", CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                            font, FontSize, Foreground, 1.25);
                var fontY = Math.Min(y + h, maxHeight) - ft.Height;
                drawingContext.DrawText(ft, new Point(x + (w - ft.Width) / 2, fontY));
            }
        }

        public void Start()
        {
            Stop();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            //if (DispatcherTimer != null)
            //{
            //    DispatcherTimer.Start();
            //    return;
            //}
            //DispatcherTimer = new DispatcherTimer
            //{
            //    Interval = new TimeSpan(0, 0, 0, 0, 50)
            //};
            //DispatcherTimer.Tick += DispatcherTimer_Tick;
            //DispatcherTimer.Start();
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            TempY += 1;
            InvalidateVisual();
        }

        private void DispatcherTimer_Tick(object? sender, EventArgs e)
        {
            TempY += 1;
            InvalidateVisual();
        }

        public void Stop()
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            // DispatcherTimer?.Stop();
        }
    }
}
