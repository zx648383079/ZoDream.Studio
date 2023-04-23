using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZoDream.Shared.Models;
using ZoDream.Shared.Utils;

namespace ZoDream.Studio.Controls
{
    public class PianoRollPanel : RollTablePanel
    {
        static PianoRollPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PianoRollPanel), new FrameworkPropertyMetadata(typeof(PianoRollPanel)));
        }




        public IList<NoteItem> ItemsSource {
            get { return (IList<NoteItem>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IList<NoteItem>), 
                typeof(PianoRollPanel), new PropertyMetadata(null, (d, s) => {
                    (d as PianoRollPanel)?.UpdateItems();
                }));



        private PianoKeyboardPanel? PianoBar;

        protected override void HeaderLoadOverride(IRollHeaderBar? bar)
        {
            PianoBar = bar as PianoKeyboardPanel;
        }

        protected override void MoveItemOverride(FrameworkElement item, double x, double y)
        {
            if (PianoBar != null && item is NoteBar o)
            {
                o.Label = PianoBar.Get(y).ToString();
            }
        }

        protected override FrameworkElement? GetContainerForItemOverride(double x, double y)
        {
            if (ItemsSource is null)
            {
                return null;
            }
            return new NoteBar()
            {
                Label = PianoBar!.Get(y).ToString()
            };
        }

        protected double FromVerticalValue(PianoKey key)
        {
            return PianoBar!.ToOffset(key);
        }


        private void AddItem(NoteItem data)
        {
            AddItem(new NoteBar()
            {
                Data = data,
                Label = data.Key.ToString(),
                Width = FromHorizontalValue(data.Duration),
            }, FromHorizontalValue(data.Begin), FromVerticalValue(data.Key));
        }

        private void UpdateItem(NoteBar item, NoteItem data)
        {
            item.Data = data;
            item.Label = data.Key.ToString();
            item.Width = FromHorizontalValue(data.Duration);
            UpdateItem(item, FromHorizontalValue(data.Begin), 
                FromVerticalValue(data.Key));
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
