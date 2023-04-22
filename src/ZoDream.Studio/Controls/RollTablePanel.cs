using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ZoDream.Shared.Utils;
using ZoDream.Studio.Extensions;

namespace ZoDream.Studio.Controls
{
    [TemplatePart(Name = TrackPanel.TrackPanelName, Type = typeof(Canvas))]
    [TemplatePart(Name = TrackPanel.RuleName, Type = typeof(RulePanel))]
    [TemplatePart(Name = TrackPanel.HorizontalBarName, Type = typeof(ScrollBar))]
    [TemplatePart(Name = TrackPanel.VerticalBarName, Type = typeof(ScrollBar))]
    [TemplatePart(Name = HeaderBarName, Type = typeof(IRollHeaderBar))]
    public class RollTablePanel : Control
    {
        public const string HeaderBarName = "PART_HeaderBar";
        static RollTablePanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RollTablePanel), new FrameworkPropertyMetadata(typeof(RollTablePanel)));
        }

        public double ItemHeight {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(RollTablePanel), new PropertyMetadata(30.0));



        public double ItemWidthGap {
            get { return (double)GetValue(ItemWidthGapProperty); }
            set { SetValue(ItemWidthGapProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemWidthGap.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemWidthGapProperty =
            DependencyProperty.Register("ItemWidthGap", typeof(double), typeof(RollTablePanel), new PropertyMetadata(10.0));



        public int MaxVerticalItemCount {
            get { return (int)GetValue(MaxVerticalItemCountProperty); }
            set { SetValue(MaxVerticalItemCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxVerticalItemCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxVerticalItemCountProperty =
            DependencyProperty.Register("MaxVerticalItemCount", typeof(int), typeof(RollTablePanel), new PropertyMetadata(134));




        protected readonly MouseHelper MouseHelper = new();
        protected IRollHeaderBar? HeaderBar;
        private Canvas? BoxPanel;
        private RulePanel? Ruler;
        private ScrollBar? HorizontalBar;
        private ScrollBar? VerticalBar;
        private double HorizontalOffset = .0;
        private double VerticalOffset = .0;
        private double LastKeyWidth = .0;
        private FrameworkElement? MoveItem;
        private MoveStatus MoveStatus = MoveStatus.None;
        private Rectangle? LineMask;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BoxPanel = GetTemplateChild(TrackPanel.TrackPanelName) as Canvas;
            Ruler = GetTemplateChild(TrackPanel.RuleName) as RulePanel;
            HorizontalBar = GetTemplateChild(TrackPanel.HorizontalBarName) as ScrollBar;
            VerticalBar = GetTemplateChild(TrackPanel.VerticalBarName) as ScrollBar;
            HeaderBar = GetTemplateChild(HeaderBarName) as IRollHeaderBar;
            if (Ruler != null)
            {
                Ruler.SizeChanged += Ruler_SizeChanged;
            }
            if (HorizontalBar != null)
            {
                HorizontalBar.Maximum = 1000;
                HorizontalBar.ValueChanged += HorizontalBar_ValueChanged;
            }
            if (VerticalBar != null)
            {
                VerticalBar.Maximum = MaxVerticalItemCount;
                VerticalBar.ValueChanged += VerticalBar_ValueChanged;
            }
            if (BoxPanel != null)
            {
                BoxPanel.MouseEnter += BoxPanel_MouseEnter;
                BoxPanel.MouseLeave += BoxPanel_MouseLeave;
                BoxPanel.MouseLeftButtonDown += BoxPanel_MouseLeftButtonDown;
                BoxPanel.MouseLeftButtonUp += BoxPanel_MouseLeftButtonUp;
                BoxPanel.MouseMove += BoxPanel_MouseMove;
                BoxPanel.MouseWheel += BoxPanel_MouseWheel;
            }
            if (HeaderBar != null)
            {
                HeaderBar.OnScroll += HeaderBar_OnScroll;
            }
            AddLineMask();
            HeaderLoadOverride(HeaderBar);
        }

        private void AddLineMask()
        {
            if (BoxPanel == null)
            {
                return;
            }
            LineMask = new Rectangle
            {
                Height = ItemHeight,
                Width = ActualWidth,
                Fill = new SolidColorBrush(Colors.AliceBlue),
                Visibility = Visibility.Collapsed
            };
            BoxPanel.Children.Add(LineMask);
            Canvas.SetTop(LineMask, 0);
            Canvas.SetLeft(LineMask, 0);
            Panel.SetZIndex(LineMask, -1);
        }

        private void MoveLineMask(double y)
        {
            if (LineMask is null || LineMask.Visibility != Visibility.Visible)
            {
                return;
            }
            LineMask.Width = ActualWidth;
            Canvas.SetTop(LineMask, GetStepChange(y - 
                HorizontalBar!.ActualHeight - Ruler!.ActualHeight, ItemHeight, true, VerticalOffset));
        }

        private void BoxPanel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (MoveItem != null)
            {
                return;
            }
            ScrollVertical(-e.Delta);
        }

        private void HeaderBar_OnScroll(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ScrollVertical(e.NewValue - VerticalOffset);
        }

        private void ScrollVertical(double offset)
        {
            if (offset == 0)
            {
                return;
            }
            var val = VerticalOffset + offset;
            if (val < 0)
            {
                val = 0;
                offset = -VerticalOffset;
            }
            VerticalOffset = val;
            HeaderBar!.Offset = VerticalOffset;
            VerticalBar!.Value = VerticalOffset / ItemHeight;
            MoveItems(0, offset);
        }

        private void ScrollHorizontal(double offset)
        {
            if (offset == 0)
            {
                return;
            }
            var val = HorizontalOffset + offset;
            if (val < 0)
            {
                val = 0;
                offset = -HorizontalOffset;
            }
            if (val + ActualWidth > HorizontalBar!.Maximum)
            {
                // HorizontalBar.Maximum += ActualWidth * 2;
            }
            HorizontalOffset = val;
            HorizontalBar!.Value = HorizontalOffset;
            Ruler!.Offset = HorizontalOffset;
            MoveItems(offset, 0);
        }

        private void BoxPanel_MouseMove(object sender, MouseEventArgs e)
        {
            var p1 = e.GetPosition(this);
            MoveLineMask(p1.Y);
            MouseHelper.MouseMove(p1);
            if (MoveItem == null || e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }
            var x = Canvas.GetLeft(MoveItem);
            var diffX = MouseHelper.OffsetX;
            var targetX = .0;
            var targetY = .0;
            switch (MoveStatus)
            {
                case MoveStatus.Move:
                    var y = Canvas.GetTop(MoveItem);
                    targetX = x + diffX;
                    if (targetX + HorizontalOffset < 0)
                    {
                        targetX = -HorizontalOffset;
                    }
                    Canvas.SetLeft(MoveItem, targetX);
                    targetY = MouseHelper.OffsetY + y;
                    if (targetY + VerticalOffset < 0)
                    {
                        targetY = - VerticalOffset;
                    }
                    Canvas.SetTop(MoveItem, targetY);
                    break;
                case MoveStatus.SizeRight:
                    MoveItem.Width = Math.Max(MoveItem.ActualWidth + diffX, ItemWidthGap);
                    break;
                case MoveStatus.SizeLeft:
                    MoveItem.Width = Math.Max(MoveItem.ActualWidth - diffX, ItemWidthGap);
                    targetX = x + diffX;
                    if (targetX + HorizontalOffset < 0)
                    {
                        targetX = 0;
                    }
                    Canvas.SetLeft(MoveItem, targetX);
                    break;
                default:
                    break;
            }
        }

        private void BoxPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(this);
            MouseHelper.MouseUp(p);
            if (MoveItem != null)
            {
                if (MoveStatus == MoveStatus.Move)
                {
                    var y = GetStepChange(Canvas.GetTop(MoveItem), 30,
                        MouseHelper.IsGlobeTop, VerticalOffset);
                    var x = GetStepChange(Canvas.GetLeft(MoveItem),
                        MouseHelper.IsGlobeLeft, HorizontalOffset);
                    if (x + HorizontalOffset < 0)
                    {
                        x = -HorizontalOffset;
                    }
                    if (y + VerticalOffset < 0)
                    {
                        x = -VerticalOffset;
                    }
                    MoveItemOverride(MoveItem, x, y);
                    Canvas.SetTop(MoveItem, y);
                    Canvas.SetLeft(MoveItem, x);
                }
                else if (MoveStatus == MoveStatus.SizeRight || MoveStatus == MoveStatus.SizeLeft)
                {
                    LastKeyWidth = MoveItem.Width = GetStepChange(MoveItem.Width, MouseHelper.IsGlobeLeft);
                }
            }
            MoveItem = null;
        }

        private void BoxPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MoveItem != null)
            {
                return;
            }
            Add(e.GetPosition(BoxPanel));
        }

        private void BoxPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
            if (LineMask is not null)
            {
                LineMask.Visibility = Visibility.Collapsed;
            }
        }

        private void BoxPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Pen;
            if (LineMask is not null)
            {
                LineMask.Visibility = Visibility.Visible;
            }
        }

        private void VerticalBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ScrollVertical(Math.Min(e.NewValue * ItemHeight, MaxVerticalItemCount * ItemHeight - ActualHeight) - VerticalOffset);
        }

        private void HorizontalBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ScrollHorizontal(e.NewValue -  HorizontalOffset);
        }

        private void Ruler_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // UpdateSize();
        }


        public void AddItem(FrameworkElement element, double x, double y)
        {
            Canvas.SetTop(element, y - VerticalOffset);
            Canvas.SetLeft(element, x - HorizontalOffset);
            BoxPanel!.Children.Add(element);
        }

        public void RemoveItem(FrameworkElement element)
        {
            BoxPanel!.Children.Add(element);
        }

        public void Clear()
        {
            if (BoxPanel is null)
            {
                return;
            }
            for (var i = BoxPanel.Children.Count - 1; i >= 0; i--)
            {
                if (BoxPanel.Children[i] == LineMask)
                {
                    continue;
                }
                BoxPanel.Children.RemoveAt(i);
            }
        }

        private void MoveItems(double hOffset, double vOffset)
        {
            if (BoxPanel is null)
            {
                return;
            }
            foreach (var item in BoxPanel.Children)
            {
                if (item == LineMask)
                {
                    if (vOffset != 0)
                    {
                        Canvas.SetTop(LineMask, Canvas.GetTop(LineMask) - vOffset);
                    }
                } else if (item is FrameworkElement o)
                {
                    if (vOffset != 0)
                    {
                        Canvas.SetTop(o, Canvas.GetTop(o) - vOffset);
                    }
                    if (hOffset != 0)
                    {
                        Canvas.SetLeft(o, Canvas.GetLeft(o) - hOffset);
                    }
                }
            }
        }

        private void Add(Point point)
        {
            if (BoxPanel == null)
            {
                return;
            }
            var y = GetStepChange(point.Y, ItemHeight, true, VerticalOffset);
            var x = GetBarLeftByMouse(point.X);
            var bar = GetContainerForItemOverride(x, y);
            if (bar is null)
            {
                return;
            }
            bar.Height = ItemHeight;
            bar.Width = GetBarWidth();
            Canvas.SetLeft(bar, x);
            Canvas.SetTop(bar, y);
            
            BoxPanel.Children.Add(bar);
            bar.MouseMove += MoveItem_MouseMove;
            bar.MouseLeftButtonDown += MoveItem_MouseLeftButtonDown;
            bar.MouseRightButtonDown += MoveItem_MouseRightButtonDown;
            bar.MouseLeave += MoveItem_MouseLeave;
        }

        /// <summary>
        /// 根据坐标创建元素
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual FrameworkElement? GetContainerForItemOverride(double x, double y)
        {
            return null;
        }

        protected virtual void MoveItemOverride(FrameworkElement item, double x, double y)
        {
            
        }

        protected virtual void HeaderLoadOverride(IRollHeaderBar? bar)
        {

        }

        private double GetBarLeft(double x)
        {
            return GetStepChange(x, ItemWidthGap, true);
        }

        private double GetBarLeftByMouse(double x)
        {
            return GetBarLeft(x + HorizontalOffset);
        }

        private double GetStepChange(double val, bool isFloor = true)
        {
            return GetStepChange(val, ItemWidthGap, isFloor);
        }

        private double GetStepChange(double val, bool isFloor, double offset)
        {
            return GetStepChange(val + offset, ItemWidthGap, isFloor) - offset;
        }

        private double GetStepChange(double val, double step, bool isFloor, double offset)
        {
            return GetStepChange(val + offset, step, isFloor) - offset;
        }

        private double GetStepChange(double val, double step, bool isFloor = true)
        {
            if (val <= 0)
            {
                return 0;
            }
            if (step <= 0)
            {
                return val;
            }
            var count = val / step;
            return (isFloor ? Math.Floor(count) : Math.Ceiling(count)) * step;
        }

        private double GetBarWidth(double width)
        {
            return GetStepChange(width, ItemWidthGap, false);
        }

        private double GetBarWidth()
        {
            return GetBarWidth(LastKeyWidth > 0 ? LastKeyWidth : 100);
        }

        private void MoveItem_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Pen;
        }

        private void MoveItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            BoxPanel!.Children.Remove((UIElement)sender);
        }

        private void MoveItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MoveItem = (FrameworkElement)sender;
            MouseHelper.MouseDown(1, e.GetPosition(this));
        }

        private void MoveItem_MouseMove(object sender, MouseEventArgs e)
        {
            if (MoveItem != null)
            {
                return;
            }
            var bar = (FrameworkElement)sender;
            var p = e.GetPosition(bar);
            if (p.X < 5)
            {
                Cursor = Cursors.SizeWE;
                MoveStatus = MoveStatus.SizeLeft;
            } else if (p.X > bar.ActualWidth - 5)
            {
                Cursor = Cursors.SizeWE;
                MoveStatus = MoveStatus.SizeRight;
            }
            else
            {
                Cursor = Cursors.SizeAll;
                MoveStatus = MoveStatus.Move;
            }
        }
    }
}
