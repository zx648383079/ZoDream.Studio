using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Linq;
using ZoDream.Shared.Models;
using ZoDream.Shared.Utils;
using ZoDream.Studio.Extensions;

namespace ZoDream.Studio.Controls
{
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

        public double ItemWidthGap {
            get { return (double)GetValue(ItemWidthGapProperty); }
            set { SetValue(ItemWidthGapProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemWidthGap.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemWidthGapProperty =
            DependencyProperty.Register("ItemWidthGap", typeof(double), typeof(TrackPanel), new PropertyMetadata(20.0));




        public IList<ProjectTrackItem>? ItemsSource
        {
            get { return (IList<ProjectTrackItem>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IList<ProjectTrackItem>), typeof(TrackPanel), 
                new PropertyMetadata(null, (d, s) => {
                    (d as TrackPanel)?.UpdateItems();
                }));



        public ICommand Command {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(TrackPanel), new PropertyMetadata(null));



        private readonly MouseHelper MouseHelper = new();
        private Canvas? BoxPanel;
        private RulePanel? Ruler;
        private ScrollBar? HorizontalBar;
        private ScrollBar? VerticalBar;
        private double HeaderWidth = 200.0;
        private double HorizontalOffset = .0;
        private double VerticalOffset = .0;
        private readonly int ItemWidthScale = 200;
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
            RefreshHorizontal();
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
            RefreshHorizontal();
        }

        /// <summary>
        /// 坐标转 offset/ms
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        protected virtual int ToHorizontalValue(double val)
        {
            return (int)((val + HorizontalOffset - HeaderWidth) / ItemWidthGap) * ItemWidthScale;
        }

        /// <summary>
        /// offset/ms 转换为 坐标 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        protected virtual double FromHorizontalValue(double val)
        {
            return val * ItemWidthGap / ItemWidthScale + HeaderWidth - HorizontalOffset;
        }

        protected virtual int ToVerticalValue(double val)
        {
            return (int)((val + VerticalOffset) / RowHeight);
        }

        protected virtual double FromVerticalValue(int val)
        {
            return val * RowHeight - VerticalOffset;
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
                    UpdateRow(row, GetTrackData(row.RowIndex));
                }
            }
        }

        private void UpdateRow(TrackHeader header)
        {
            header.Width = HeaderWidth;
            header.Data.Index = header.RowIndex;
            Canvas.SetTop(header, FromVerticalValue(header.RowIndex));
        }

        private void UpdateRow(TrackBar row)
        {
            // Canvas.SetLeft(row, HeaderWidth - HorizontalOffset);
            Canvas.SetTop(row, FromVerticalValue(row.RowIndex));
        }

        private void UpdateRow(TrackHeader header, ProjectTrackItem data)
        {
            header.Width = HeaderWidth;
            header.Data = data;
            Canvas.SetTop(header, FromVerticalValue(header.RowIndex));
        }

        private void UpdateRow(TrackBar row, ProjectTrackItem? data)
        {
            if (data is not null)
            {
                Canvas.SetLeft(row, FromHorizontalValue(data.Offset));
            }
            Canvas.SetTop(row, FromVerticalValue(row.RowIndex));
        }

        private ProjectTrackItem? GetTrackData(int index)
        {
            if (ItemsSource is null)
            {
                return null;
            }
            foreach (var item in ItemsSource)
            {
                if (item.Index == index)
                {
                    return item;
                }
            }
            return null;
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
            ProjectTrackItem? data = GetTrackData(index);
            if (data is null)
            {
                return;
            }
            data.Index = to;
            data.Offset = ToHorizontalValue(x);
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
                        Debug.WriteLine(header.RowIndex);
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
                        Canvas.SetLeft(row, FromHorizontalValue(data.Offset));
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

        private static bool InRange(int i, int min, int max)
        {
            if (min > max)
            {
                return i <= min && i >= max;
            }
            return i <= max && i >= min;
        }

        protected void EachChildren(Action<TrackHeader, TrackBar, int> fn)
        {
            if (BoxPanel is null)
            {
                return;
            }
            var exist = new List<int>();
            for (var i = BoxPanel.Children.Count - 1; i >= 0; i--)
            {
                var item = BoxPanel.Children[i];
                if (item is TrackBar b && exist.IndexOf(b.RowIndex) < 0)
                {
                    exist.Add(i);
                    fn.Invoke((TrackHeader)EachGetItem(i, b.RowIndex, true)!, b, b.RowIndex);
                    continue;
                }
                if (item is TrackHeader h && exist.IndexOf(h.RowIndex) < 0)
                {
                    exist.Add(i);
                    fn.Invoke(h, (TrackBar)EachGetItem(i, h.RowIndex, false)!, h.RowIndex);
                    continue;
                }
            }
        }

        private FrameworkElement? EachGetItem(int j, int row, bool isHeader = false)
        {
            for (var i = j - 1; i >= 0; i--)
            {
                var item = BoxPanel!.Children[i];
                if (item is TrackBar b && !isHeader && b.RowIndex == row)
                {
                    return b;
                }
                if (item is TrackHeader h && isHeader && h.RowIndex == row)
                {
                    return h;
                }
            }
            return null;
        }

        public void Clear()
        {
            if (BoxPanel is null)
            {
                return;
            }
            for (var i = BoxPanel.Children.Count - 1; i >= 0; i--)
            {
                var item = BoxPanel.Children[i];
                if (item is TrackBar || item is TrackHeader)
                {
                    BoxPanel.Children.RemoveAt(i);
                }
            }
        }

        private void UpdateItems()
        {
            if (ItemsSource == null)
            {
                Clear();
                return;
            }
            if (ItemsSource is INotifyCollectionChanged o)
            {
                o.CollectionChanged -= Items_CollectionChanged;
                o.CollectionChanged += Items_CollectionChanged;
            }
            if (ItemsSource is INotifyPropertyChanged i)
            {
                i.PropertyChanged -= Items_PropertyChanged;
                i.PropertyChanged += Items_PropertyChanged;
            }
            RefreshItems();
        }

        private void Items_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // RefreshItems();
        }

        private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshItems();
        }

        private void RefreshHorizontal()
        {
            if (HorizontalBar is null || ItemsSource is null)
            {
                return;
            }
            var len = ProjectItem.GetDurationMilliseconds(ItemsSource) / ItemWidthScale + 50;
            if (HorizontalBar.Maximum < len)
            {
                HorizontalBar.Maximum = len;
            }
        }

        private void RefreshItems()
        {
            var items = ItemsSource!.ToList();
            RefreshHorizontal();
            var targetItems = new List<TrackHeader>();
            var targetBarItems = new List<TrackBar>();
            EachChildren((header, bar, index) => {
                if (bar is null)
                {
                    return;
                }
                for (var j = items.Count - 1; j >= 0; j--)
                {
                    if (items[j].Index == index)
                    {
                        UpdateRow(header, items[j]);
                        UpdateRow(bar, items[j]);
                        items.RemoveAt(j);
                        return;
                    }
                }
                targetItems.Add(header);
                targetBarItems.Add(bar);
            });
            for (int j = items.Count - 1; j >= 0; j--)
            {
                if (targetItems.Count == 0)
                {
                    AddRow(items[j]);
                    continue;
                }
                var last = targetItems.Count - 1;
                UpdateRow(targetItems[last], items[j]);
                UpdateRow(targetBarItems[last], items[j]);
                targetItems.RemoveAt(last);
                targetBarItems.RemoveAt(last);
            }
            foreach (var item in targetItems)
            {
                RemoveRow(item);
            }
            foreach (var item in targetBarItems)
            {
                RemoveRow(item);
            }
        }

        private void RemoveRow(FrameworkElement item)
        {
            BoxPanel!.Children.Remove(item);
        }


        private void AddRow(ProjectTrackItem data)
        {
            var y = data.Index * RowHeight - VerticalOffset;
            var header = new TrackHeader
            {
                Width = HeaderWidth,
                Height = RowHeight,
                RowIndex = data.Index,
                Data = data,
                Command = Command
            };
            Panel.SetZIndex(header, 66);
            Canvas.SetLeft(header, 0);
            Canvas.SetTop(header, y);
            BoxPanel!.Children.Add(header);
            var row = new TrackBar()
            {
                Height = RowHeight,
                Width = 200,
                RowIndex = data.Index,
            };
            Panel.SetZIndex(row, 0);
            Canvas.SetLeft(row, HeaderWidth - HorizontalOffset);
            Canvas.SetTop(row, y);
            BoxPanel!.Children.Add(row);
            row.MouseLeftButtonDown += Row_MouseLeftButtonDown;
        }
    }
}
