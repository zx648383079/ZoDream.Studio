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
    ///     <MyNamespace:RulePanel/>
    ///
    /// </summary>
    public class RulePanel : Control
    {
        static RulePanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RulePanel), new FrameworkPropertyMetadata(typeof(RulePanel)));
        }

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(RulePanel), 
                new PropertyMetadata(Orientation.Horizontal));



        public double Offset
        {
            get { return (double)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Offset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(double), typeof(RulePanel), 
                new PropertyMetadata(.0, (d, e) =>
            {
                (d as RulePanel)?.InvalidateVisual();
            }));




        public double Gap
        {
            get { return (double)GetValue(GapProperty); }
            set { SetValue(GapProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Gap.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GapProperty =
            DependencyProperty.Register("Gap", typeof(double), typeof(RulePanel), 
                new PropertyMetadata(10.0, (d, e) =>
                {
                    (d as RulePanel)?.InvalidateVisual();
                }));



        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            var count = Math.Ceiling((Orientation == Orientation.Horizontal ? ActualWidth : ActualHeight) / Gap);
            var start = Math.Ceiling(Offset / Gap);
            var h = Orientation == Orientation.Horizontal ? ActualHeight : ActualWidth;
            var pen = new Pen(Foreground, 1);
            var font = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
            for (int i = 0; i < count; i++)
            {
                var real = start + i;
                var HasLabel = real % 5 == 0;
                var x = real * Gap - Offset;
                var y = h * (HasLabel ? .6 : .4);
                if (Orientation == Orientation.Horizontal)
                {
                    drawingContext.DrawLine(pen, new Point(x, 0), new Point(x, y));
                    if (HasLabel)
                    {
                        drawingContext.DrawText(new FormattedText(
                            real.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                            font, h * .3, Foreground, 1.25
                            ), new Point(x, y));
                    }
                } else
                {
                    drawingContext.DrawLine(pen, new Point(0, x), new Point(y, x));
                    if (HasLabel)
                    {
                        drawingContext.DrawText(new FormattedText(
                            real.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                            font, h * .3, Foreground, 1.25
                            ), new Point(y, x));
                    }
                }
            }
        }
    }
}
