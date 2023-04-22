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
using ZoDream.Shared.Utils;
using ZoDream.Studio.Extensions;

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
    ///     <MyNamespace:RangeProgressBar/>
    ///
    /// </summary>
    public class RangeProgressBar : Control
    {
        static RangeProgressBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RangeProgressBar), new FrameworkPropertyMetadata(typeof(RangeProgressBar)));
        }




        public int Max {
            get { return (int)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Max.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(int), typeof(RangeProgressBar), new PropertyMetadata(0));




        public int Value {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(RangeProgressBar), 
                new PropertyMetadata(0, (d, s) => {
                    (d as RangeProgressBar)?.InvalidateVisual();
                }));




        public int Begin {
            get { return (int)GetValue(BeginProperty); }
            set { SetValue(BeginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Begin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BeginProperty =
            DependencyProperty.Register("Begin", typeof(int), typeof(RangeProgressBar), 
                new PropertyMetadata(0, (d, s) => {
                    (d as RangeProgressBar)?.InvalidateVisual();
                }));



        public int End {
            get { return (int)GetValue(EndProperty); }
            set { SetValue(EndProperty, value); }
        }

        // Using a DependencyProperty as the backing store for End.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndProperty =
            DependencyProperty.Register("End", typeof(int), typeof(RangeProgressBar), 
                new PropertyMetadata(0, (d, s) => {
                    (d as RangeProgressBar)?.InvalidateVisual();
                }));




        public bool RangeVisible {
            get { return (bool)GetValue(RangeVisibleProperty); }
            set { SetValue(RangeVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeVisibleProperty =
            DependencyProperty.Register("RangeVisible", typeof(bool), 
                typeof(RangeProgressBar), new PropertyMetadata(false, (d, s) => {
                    (d as RangeProgressBar)?.InvalidateVisual();
                }));




        public Color ActiveColor {
            get { return (Color)GetValue(ActiveColorProperty); }
            set { SetValue(ActiveColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActiveColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActiveColorProperty =
            DependencyProperty.Register("ActiveColor", typeof(Color), typeof(RangeProgressBar), new PropertyMetadata(Colors.Black));




        public Color OutColor {
            get { return (Color)GetValue(OutColorProperty); }
            set { SetValue(OutColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OutColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OutColorProperty =
            DependencyProperty.Register("OutColor", typeof(Color), typeof(RangeProgressBar), new PropertyMetadata(Colors.Gray));




        public double BarSize {
            get { return (double)GetValue(BarSizeProperty); }
            set { SetValue(BarSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BarSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BarSizeProperty =
            DependencyProperty.Register("BarSize", typeof(double), typeof(RangeProgressBar), new PropertyMetadata(10.0));



        public Color PointerColor {
            get { return (Color)GetValue(PointerColorProperty); }
            set { SetValue(PointerColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PointerColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PointerColorProperty =
            DependencyProperty.Register("PointerColor", typeof(Color), typeof(RangeProgressBar), new PropertyMetadata(Colors.Orange));



        public double PointerThickness {
            get { return (double)GetValue(PointerThicknessProperty); }
            set { SetValue(PointerThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PointerThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PointerThicknessProperty =
            DependencyProperty.Register("PointerThickness", typeof(double), typeof(RangeProgressBar), new PropertyMetadata(2.0));




        public ICommand Command {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(RangeProgressBar), new PropertyMetadata(null));


        private MouseHelper MouseHelper = new();


        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            DrawBar(drawingContext);
            DrawPointer(drawingContext);
        }

        private void DrawBar(DrawingContext context)
        {
            var width = ActualWidth;
            var y = (ActualHeight - BarSize) / 2;
            var activeBrush = new SolidColorBrush(ActiveColor);
            var pen = new Pen();
            if (!RangeVisible)
            {
                context.DrawRectangle(activeBrush, pen, new Rect(
                    0, y,
                    width,
                    BarSize
                    ));
                return;
            }
            context.DrawRectangle(new SolidColorBrush(OutColor), pen, new Rect(
                    0, y,
                    width,
                    BarSize
                    ));
            if (Begin > End || Max <= 0)
            {
                return;
            }
            var beginX = Math.Min(Begin * width / Max, width);
            var endX = Math.Min(End * width / Max, width);
            if (beginX > endX)
            {
                return;
            }
            context.DrawRectangle(activeBrush, pen, new Rect(
                    beginX, y,
                    Math.Max(endX - beginX, 1),
                    BarSize
                    ));
        }

        private void DrawPointer(DrawingContext context)
        {
            var height = ActualHeight;
            if (Max <= 0)
            {
                return;
            }
            var width = ActualWidth - PointerThickness;
            var x = Value * width / Max;
            context.DrawRectangle(new SolidColorBrush(PointerColor), new Pen(), new Rect(
                    x, 0,
                    PointerThickness,
                    height
                    ));
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            MouseHelper.MouseDown(e.GetPosition(this));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            MouseHelper.MouseMove(e.GetPosition(this));
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            MouseHelper.MouseUp(e.GetPosition(this));
            if (Max <= 0)
            {
                return;
            }
            if (!MouseHelper.IsTouchMove)
            {
                Value = Math.Min(Max, (int)Math.Floor(MouseHelper.CurrentX * Max / (ActualWidth - PointerThickness)));
            } else
            {
                var val = MouseHelper.TotalOffsetX * Max / (ActualWidth - PointerThickness) + Value;
                Value = Math.Min(Max, (int)(MouseHelper.IsGlobeLeft ? Math.Floor(val) : Math.Ceiling(val)));
            }
            Command?.Execute(Value);
        }
    }
}
