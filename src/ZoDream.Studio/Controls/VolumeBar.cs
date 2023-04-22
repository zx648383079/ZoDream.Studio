using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
    ///     <MyNamespace:VolumeBar/>
    ///
    /// </summary>
    public class VolumeBar : Control, IRollHeaderBar
    {
        static VolumeBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VolumeBar), new FrameworkPropertyMetadata(typeof(VolumeBar)));
        }



        public double Offset {
            get { return (double)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Offset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(double), typeof(VolumeBar), new PropertyMetadata(0.0, (d, s) => {
                (d as VolumeBar)?.InvalidateVisual();
            }));



        public int Max {
            get { return (int)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Max.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(int), typeof(VolumeBar), new PropertyMetadata(127));



        public double KeySize {
            get { return (double)GetValue(KeySizeProperty); }
            set { SetValue(KeySizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for KeySize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeySizeProperty =
            DependencyProperty.Register("KeySize", typeof(double), typeof(VolumeBar), new PropertyMetadata(30.0));



        public event RoutedPropertyChangedEventHandler<double>? OnScroll;

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            var pen = new Pen(Foreground, FontSize);
            var linePen = new Pen(BorderBrush, BorderThickness.Bottom);
            var font = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
            var y = Padding.Top;
            var width = ActualWidth;
            EachLine((label, y) => {
                var formatted = new FormattedText(label, CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight, font, FontSize, Foreground, 1.25);
                drawingContext.DrawText(
                    formatted
                    , new Point(
                        (width - formatted.Width)/ 2,
                        y + (KeySize - formatted.Height) / 2
                        )
                    );
                var endY = y + KeySize;
                drawingContext.DrawLine(linePen, new Point(0, endY), 
                    new Point(width, endY));
            });
        }

        private void EachLine(Action<string, double> fn)
        {
            var height = ActualHeight;
            var offset = Math.Abs(Offset);
            var begin = Math.Floor(offset / KeySize);
            var count = Math.Ceiling(height / KeySize);
            for (var i = 0; i < count; i++)
            {
                var j = begin + i;
                var val = Max - j;
                fn.Invoke(val.ToString(), j * KeySize - offset);
            }
        }
    }
}
