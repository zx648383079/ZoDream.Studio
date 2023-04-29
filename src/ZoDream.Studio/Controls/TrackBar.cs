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
    ///     <MyNamespace:TrackBar/>
    ///
    /// </summary>
    public class TrackBar : Control
    {
        static TrackBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TrackBar), new FrameworkPropertyMetadata(typeof(TrackBar)));
        }



        public double Begin {
            get { return (double)GetValue(BeginProperty); }
            set { SetValue(BeginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Begin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BeginProperty =
            DependencyProperty.Register("Begin", typeof(double), typeof(TrackBar), new PropertyMetadata(.0));



        public double End {
            get { return (double)GetValue(EndProperty); }
            set { SetValue(EndProperty, value); }
        }

        // Using a DependencyProperty as the backing store for End.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndProperty =
            DependencyProperty.Register("End", typeof(double), typeof(TrackBar), new PropertyMetadata(.0));

        public int RowIndex { get; set; }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            var pen = new Pen(BorderBrush, BorderThickness.Top);
            var begin = Begin;
            var end = End;
            if (end < begin || end == 0)
            {
                end = ActualWidth;
            }
            if (begin > 0 || end <  ActualWidth)
            {
                drawingContext.DrawRectangle(Background, pen, new Rect(0, 0, ActualWidth, ActualHeight));
            }
            drawingContext.DrawRectangle(Foreground, pen, new Rect(begin, 0, end, ActualHeight));
        }

    }
}
