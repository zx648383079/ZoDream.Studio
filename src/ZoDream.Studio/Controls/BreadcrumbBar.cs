using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    ///     <MyNamespace:BreadcrumbBar/>
    ///
    /// </summary>
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(BreadcrumbBarItem))]
    public class BreadcrumbBar : ItemsControl
    {
        static BreadcrumbBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BreadcrumbBar), new FrameworkPropertyMetadata(typeof(BreadcrumbBar)));
        }

        

        public ICommand Command {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(BreadcrumbBar), new PropertyMetadata(null));


        public BreadcrumbBar()
        {
            Loaded += BreadcrumbBar_Loaded;
            Unloaded += BreadcrumbBar_Unloaded;
        }

        private void BreadcrumbBar_Unloaded(object sender, RoutedEventArgs e)
        {
            ItemContainerGenerator.ItemsChanged -= ItemContainerGenerator_ItemsChanged;
            ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
        }

        private void BreadcrumbBar_Loaded(object sender, RoutedEventArgs e)
        {
            ItemContainerGenerator.ItemsChanged += ItemContainerGenerator_ItemsChanged;
            ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        private void ItemContainerGenerator_StatusChanged(object? sender, EventArgs e)
        {
            if (ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return;

            if (ItemContainerGenerator.Items.Count <= 1)
            {
                UpdateLastContainer();
                return;
            }

            InteractWithItemContainer(2, static item => item.SymbolIconVisible = Visibility.Visible);
            UpdateLastContainer();
        }

        private void ItemContainerGenerator_ItemsChanged(object sender, System.Windows.Controls.Primitives.ItemsChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Remove)
                return;

            UpdateLastContainer();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is BreadcrumbBarItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var item = new BreadcrumbBarItem();
            item.MouseDown += Item_MouseDown;
            return item;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (item is BreadcrumbBarItem o)
            {
                o.MouseDown -= Item_MouseDown;
                o.MouseDown += Item_MouseDown;
            }
        }


        private void Item_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is BreadcrumbBarItem o && o.SymbolIconVisible == Visibility.Collapsed)
            {
                return;
            }
            Command?.Execute(sender);
        }

        private void InteractWithItemContainer(int offsetFromEnd, Action<BreadcrumbBarItem> action)
        {
            if (ItemContainerGenerator.Items.Count <= 0)
                return;

            var item = ItemContainerGenerator.Items[^offsetFromEnd];
            var container = (BreadcrumbBarItem)ItemContainerGenerator.ContainerFromItem(item);

            action.Invoke(container);
        }

        private void UpdateLastContainer() => InteractWithItemContainer(1, static item => item.SymbolIconVisible = Visibility.Collapsed);
    }
}
