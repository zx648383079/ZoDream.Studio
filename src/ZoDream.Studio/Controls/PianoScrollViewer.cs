using Melanchall.DryWetMidi.Interaction;
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
using ZoDream.Shared.Models;

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




        public double KeySize {
            get { return (double)GetValue(KeySizeProperty); }
            set { SetValue(KeySizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for KeySize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeySizeProperty =
            DependencyProperty.Register("KeySize", typeof(double), typeof(PianoScrollViewer), new PropertyMetadata(30.0));



        public IEnumerable<NoteItem> ItemsSource {
            get { return (IEnumerable<NoteItem>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<NoteItem>), typeof(PianoScrollViewer), new PropertyMetadata(null));



        public PianoKeyboardPanel TargetKeyboard {
            get { return (PianoKeyboardPanel)GetValue(TargetKeyboardProperty); }
            set { SetValue(TargetKeyboardProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TargetKeyboard.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetKeyboardProperty =
            DependencyProperty.Register("TargetKeyboard", typeof(PianoKeyboardPanel), typeof(PianoScrollViewer), new PropertyMetadata(null));



        private PianoKeyScrollItem[] NoteItems = Array.Empty<PianoKeyScrollItem>();
        // private DispatcherTimer? DispatcherTimer;
        private double Offset = 0;
        private readonly double OffsetToYScale = 5;

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            var pen = new Pen(NoteBackground, 1);
            var font = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
            var maxHeight = ActualHeight;
            var maxWidth = ActualWidth;
            var endCount = 0;
            foreach (var item in NoteItems)
            {
                DrawKey(drawingContext, item, maxWidth, maxHeight, pen, font);
                if (item.Status == KeyStates.Toggled)
                {
                    endCount++;
                }
            }
            if (endCount >= NoteItems.Length)
            {
                Stop();
            }
        }

        private void DrawKey(DrawingContext context, PianoKeyScrollItem data, double maxWidth, double maxHeight, Pen pen, Typeface font)
        {
            if (data.Status == KeyStates.Toggled)
            {
                return;
            }
            if (Offset > data.Data.End)
            {
                TargetKeyboard?.Release(data.Data.Key);
                // 释放事件
                data.Status = KeyStates.Toggled;
                return;
            }
            var note = data.Data;
            var h = note.Duration * OffsetToYScale;
            var y = maxHeight + (Offset - note.Begin) * OffsetToYScale;
            var bottomY = y + h;
            if (bottomY < 0)
            {
                return;
            }
            if (bottomY >= maxHeight && data.Status == KeyStates.None)
            {
                TargetKeyboard?.Press(data.Data.Key);
                // 按下事件
                data.Status = KeyStates.Down;
            }
            var x = TargetKeyboard?.GetKeyPosition(note.Key) ?? 0;
            var w = KeySize;
            if (x > maxWidth || x + w < 0)
            {
                return;
            }
            context.DrawRectangle(NoteBackground, pen, new Rect(x, y, w, h));
            var ft = new FormattedText(
                        note.Key.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                        font, FontSize, Foreground, 1.25);
            var fontY = Math.Min(y + h, maxHeight) - ft.Height;
            context.DrawText(ft, new Point(x + (w - ft.Width) / 2, fontY));
        }

        public void Start(int offset)
        {
            Stop();
            if (ItemsSource is null)
            {
                return;
            }
            Offset = offset;
            NoteItems = ItemsSource.Select(item => new PianoKeyScrollItem(item)).ToArray();
            if (NoteItems.Length < 0)
            {
                return;
            }
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            //if (DispatcherTimer != null)
            //{
            //    DispatcherTimer.Start();
            //    return;
            //}
            //DispatcherTimer = new DispatcherTimer
            //{
            //    Interval = new TimeSpan(0, 0, 0, 0, 500)
            //};
            //DispatcherTimer.Tick += DispatcherTimer_Tick;
            //DispatcherTimer.Start();
        }

        public void Start()
        {
            Start(0);
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            OnTimerTick();
        }

        private void DispatcherTimer_Tick(object? sender, EventArgs e)
        {
            OnTimerTick();
        }

        private void OnTimerTick()
        {
            Offset += .2;
            InvalidateVisual();
        }

        public void Stop()
        {
            // CompositionTarget.Rendering -= CompositionTarget_Rendering;
            // DispatcherTimer?.Stop();
        }
    }

    class PianoKeyScrollItem
    {
        public PianoKeyScrollItem(NoteItem data)
        {
            Data = data;
        }
        public NoteItem Data { get; set; }

        public KeyStates Status { get; set; }
    }
}
