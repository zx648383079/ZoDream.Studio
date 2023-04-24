using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ZoDream.Shared.Models;

namespace ZoDream.Studio.Controls
{
    public class WordRollPanel : RollTablePanel
    {
        static WordRollPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WordRollPanel), new FrameworkPropertyMetadata(typeof(WordRollPanel)));
        }



        public ICommand AddCommand {
            get { return (ICommand)GetValue(AddCommandProperty); }
            set { SetValue(AddCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddCommandProperty =
            DependencyProperty.Register("AddCommand", typeof(ICommand), typeof(WordRollPanel), new PropertyMetadata(null));

        public IEnumerable<TextPromptItem>? ItemsSource {
            get => GetValue(ItemsSourceProperty) as IEnumerable<TextPromptItem>;
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<TextPromptItem>), 
                typeof(WordRollPanel), new PropertyMetadata(null, (d, s) => {
                    (d as WordRollPanel)?.UpdateItems();
                }));



        private VolumeBar? VolumeBar;
        private readonly int ItemWidthScale = 200;

        protected override void HeaderLoadOverride(IRollHeaderBar? bar)
        {
            VolumeBar = bar as VolumeBar;
        }

        protected override void MoveItemOverride(FrameworkElement item, double x, double y)
        {
            if (item is NoteBar o && o.Data is TextPromptItem data)
            {
                data.Offset = ToHorizontalValue(x);
                data.Volume = ToVerticalValue(y);
            }
        }

        protected override FrameworkElement? GetContainerForItemOverride(double x, double y, double width)
        {
            AddCommand?.Execute(new Point(
                ToHorizontalValue(x)
                , ToVerticalValue(y)));
            return null;
        }

        protected override void ResizeItemOverride(FrameworkElement item, double width, double x)
        {
            if (item is NoteBar o && o.Data is TextPromptItem data)
            {
                data.Offset = ToHorizontalValue(x);
                data.Duration = ToHorizontalValue(width);
            }
        }

        protected override int ToHorizontalValue(double val)
        {
            return base.ToHorizontalValue(val) * ItemWidthScale;
        }

        protected override double FromHorizontalValue(int val)
        {
            return base.FromHorizontalValue(val / ItemWidthScale);
        }

        private void AddItem(TextPromptItem data)
        {
            AddItem(new NoteBar()
            {
                Data = data,
                Label = data.Content,
                Width = FromHorizontalValue(data.Duration),
            }, FromHorizontalValue(data.Offset), FromVerticalValue(data.Volume));
        }

        private void UpdateItem(NoteBar item, TextPromptItem data)
        {
            item.Data = data;
            item.Label = data.Content;
            item.Width = FromHorizontalValue(data.Duration);
            UpdateItem(item, FromHorizontalValue(data.Offset), FromVerticalValue(data.Volume));
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
            RefreshItems();
        }

        private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshItems();
        }

        private void RefreshItems()
        {
            var items = ItemsSource!.ToList();
            var targetItems = new List<NoteBar>();
            EachChildren<NoteBar>((item, i) => {
                for (var j = items.Count - 1; j >= 0; j--)
                {
                    if (items[j] == item.Data)
                    {
                        UpdateItem(item, items[j]);
                        items.RemoveAt(j);
                        return;
                    }
                }
                targetItems.Add(item);
            });
            for (int j = items.Count - 1; j >= 0; j--)
            {
                if (targetItems.Count == 0)
                {
                    AddItem(items[j]);
                    continue;
                }
                var last = targetItems.Count - 1;
                UpdateItem(targetItems[last], items[j]);
                targetItems.RemoveAt(last);
            }
            foreach (var item in targetItems)
            {
                RemoveItem(item);
            }
        }
    }
}
