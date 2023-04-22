using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ZoDream.Shared.Models;
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
    ///     <MyNamespace:TrackPanel/>
    ///
    /// </summary>
    [TemplatePart(Name = TrackPanelName, Type =typeof(Canvas))]
    [TemplatePart(Name = RuleName, Type =typeof(RulePanel))]
    [TemplatePart(Name = HorizontalBarName, Type =typeof(ScrollBar))]
    [TemplatePart(Name = VerticalBarName, Type =typeof(ScrollBar))]
    public class TrackPanel : Control
    {
        public const string TrackPanelName = "PART_TrackPanel";
        public const string RuleName = "PART_Rule";
        public const string HorizontalBarName = "PART_HorizontalBar";
        public const string VerticalBarName = "PART_VerticalBar";
        static TrackPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TrackPanel), new FrameworkPropertyMetadata(typeof(TrackPanel)));
        }



        public double RowHeight
        {
            get { return (double)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.Register("RowHeight", typeof(double), typeof(TrackPanel), new PropertyMetadata(30.0));



        public IList<TrackItem> ItemsSource
        {
            get { return (IList<TrackItem>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IList<TrackItem>), typeof(TrackPanel), new PropertyMetadata(null));

        private readonly MouseHelper MouseHelper = new();
        private Canvas? BoxPanel;
        private RulePanel? Ruler;
        private ScrollBar? HorizontalBar;
        private ScrollBar? VerticalBar;
        private double HeaderWidth = 200.0;
        private double HorizontalOffset = .0;
        private double VerticalOffset = .0;
        private TrackBar? MoveBar;


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BoxPanel = GetTemplateChild(TrackPanelName) as Canvas;
            Ruler = GetTemplateChild(RuleName) as RulePanel;
            HorizontalBar = GetTemplateChild(HorizontalBarName) as ScrollBar;
            VerticalBar = GetTemplateChild(VerticalBarName) as ScrollBar;
            if (Ruler != null)
            {
                Ruler.SizeChanged += Ruler_SizeChanged;
            }
            if (HorizontalBar != null)
            {
                HorizontalBar.Maximum = 100;
                HorizontalBar.ValueChanged += HorizontalBar_ValueChanged;
            }
            if (VerticalBar != null)
            {
                VerticalBar.Maximum = 100;
                VerticalBar.ValueChanged += VerticalBar_ValueChanged;
            }
            for (int i = 0; i < 1; i++)
            {
                Add(i);
            }
        }

        private void VerticalBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VerticalOffset = e.NewValue;
            UpdateSize();
        }

        private void HorizontalBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            HorizontalOffset = e.NewValue;
            Ruler!.Offset = e.NewValue;
            UpdateSize();
        }

        private void Ruler_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            HeaderWidth = BoxPanel!.ActualWidth - Ruler!.ActualWidth;
            UpdateSize();
        }

        private void Add(int index)
        {
            var y = index * RowHeight - VerticalOffset;
            var header = new TrackHeader
            {
                Width = HeaderWidth,
                Height = RowHeight,
                RowIndex = index,
            };
            Panel.SetZIndex(header, 66);
            Canvas.SetLeft(header, 0);
            Canvas.SetTop(header, y);
            BoxPanel!.Children.Add(header);
            var row = new TrackBar()
            {
                Height = RowHeight,
                Width = 200,
                RowIndex = index,
            };
            Panel.SetZIndex(row, 0);
            Canvas.SetLeft(row, HeaderWidth - HorizontalOffset);
            Canvas.SetTop(row, y);
            BoxPanel!.Children.Add(row);
            row.MouseLeftButtonDown += Row_MouseLeftButtonDown;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var p = e.GetPosition(this);
            MouseHelper.MouseMove(p);
            if (MoveBar == null)
            {
                return;
            }
            var x = Canvas.GetLeft(MoveBar);
            var y = Canvas.GetTop(MoveBar);
            Canvas.SetLeft(MoveBar, Math.Max(MouseHelper.OffsetX + x, HeaderWidth));
            Canvas.SetTop(MoveBar, MouseHelper.OffsetY + y);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            MouseHelper.MouseUp(e.GetPosition(this));
            if (MoveBar == null)
            {
                return;
            }
            Cursor = Cursors.Arrow;
            MoveBar.Opacity = 1;
            MoveRow(MoveBar.RowIndex, Canvas.GetLeft(MoveBar), Canvas.GetTop(MoveBar));
            MoveBar = null;
        }

        private void Row_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MouseHelper.MouseDown(1, e.GetPosition(this));
            MoveBar = (TrackBar)sender;
            Cursor = Cursors.SizeAll;
            MoveBar.Opacity = .5;
        }


        private void UpdateSize()
        {
            if (BoxPanel == null)
            {
                return;
            }
            foreach (var item in BoxPanel.Children)
            {
                if (item == null)
                {
                    continue;
                }
                if (item is TrackHeader header)
                {
                    UpdateRow(header);
                    continue;
                }
                if (item is TrackBar row)
                {
                    UpdateRow(row);
                }
            }
        }

        private void UpdateRow(TrackHeader header)
        {
            header.Width = HeaderWidth;
            Canvas.SetTop(header, header.RowIndex * RowHeight - VerticalOffset);
        }

        private void UpdateRow(TrackBar row)
        {
            Canvas.SetLeft(row, HeaderWidth - HorizontalOffset);
            Canvas.SetTop(row, row.RowIndex * RowHeight - VerticalOffset);
        }

        private void MoveRow(int index, double x, double y)
        {
            var count = (y + VerticalOffset) / RowHeight;
            var to = Math.Min(
                (int)Math.Max(MouseHelper.IsGlobeTop ? Math.Floor(count) : Math.Ceiling(count), 0), GetMaxRow());
            //if (index == to)
            //{
            //    return;
            //}
            var toY = y + HorizontalOffset - HeaderWidth;
            foreach (var item in BoxPanel!.Children)
            {
                if (item is TrackHeader header)
                {
                    if (header.RowIndex == index)
                    {
                        header.RowIndex = to;
                    } else if (InRange(header.RowIndex, index, to))
                    {
                        header.RowIndex += (index < to ? -1 : 1);
                    } else
                    {
                        continue;
                    }
                    UpdateRow(header);
                    continue;
                }
                if (item is TrackBar row)
                {
                    if (row.RowIndex == index)
                    {
                        row.RowIndex = to;
                        // row.y = toY;
                    }
                    else if (InRange(row.RowIndex, index, to))
                    {
                        row.RowIndex += (index < to ? -1 : 1);
                    }
                    else
                    {
                        continue;
                    }
                    UpdateRow(row);
                }
            }
        }

        private int GetMaxRow()
        {
            var max = 0;
            foreach (var item in BoxPanel!.Children)
            {
                if (item is TrackHeader header && header.RowIndex > max)
                {
                    max = header.RowIndex;
                }
            }
            return max;
        }

        private bool InRange(int i, int min, int max)
        {
            if (min > max)
            {
                return i < min && i > max;
            }
            return i < max && i > min;
        }
    }
}
