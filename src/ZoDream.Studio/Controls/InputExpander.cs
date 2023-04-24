using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
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
    ///     <MyNamespace:InputExpander/>
    ///
    /// </summary>
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(InputCard))]
    [TemplatePart(Name = HeaderBtnName, Type = typeof(FrameworkElement))]
    public class InputExpander : ItemsControl
    {
        const string HeaderBtnName = "PART_HeaderBtn";
        static InputExpander()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(InputExpander), new FrameworkPropertyMetadata(typeof(InputExpander)));
        }

        public string Icon {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(InputExpander),
                new PropertyMetadata(string.Empty, (d, s) => {
                    if (d is InputExpander box)
                    {
                        box.IconVisible = string.IsNullOrWhiteSpace(box.Icon) ? Visibility.Collapsed : Visibility.Visible;
                    }
                }));



        public string Header {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(InputExpander), new PropertyMetadata(string.Empty));




        public string Text {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(InputExpander), new PropertyMetadata(string.Empty));




        public string Description {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(InputExpander), new PropertyMetadata(string.Empty, (d, s) => {
                if (d is InputExpander box)
                {
                    box.MetaVisible = string.IsNullOrWhiteSpace(box.Description) ? Visibility.Collapsed : Visibility.Visible;
                }
            }));



        public string ActionIcon {
            get { return (string)GetValue(ActionIconProperty); }
            set { SetValue(ActionIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActionIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActionIconProperty =
            DependencyProperty.Register("ActionIcon", typeof(string), typeof(InputExpander), new PropertyMetadata("\uE70D"));



        public Visibility IconVisible {
            get { return (Visibility)GetValue(IconVisibleProperty); }
            set { SetValue(IconVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconVisibleProperty =
            DependencyProperty.Register("IconVisible", typeof(Visibility), typeof(InputExpander), new PropertyMetadata(Visibility.Collapsed));




        public Visibility MetaVisible {
            get { return (Visibility)GetValue(MetaVisibleProperty); }
            set { SetValue(MetaVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MetaVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MetaVisibleProperty =
            DependencyProperty.Register("MetaVisible", typeof(Visibility), typeof(InputExpander), new PropertyMetadata(Visibility.Collapsed));




        public CornerRadius CornerRadius {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(InputExpander), new PropertyMetadata(null));



        public CornerRadius HeaderCornerRadius {
            get { return (CornerRadius)GetValue(HeaderCornerRadiusProperty); }
            set { SetValue(HeaderCornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderCornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderCornerRadiusProperty =
            DependencyProperty.Register("HeaderCornerRadius", typeof(CornerRadius), typeof(InputExpander), new PropertyMetadata(null));



        public Visibility ItemsVisible {
            get { return (Visibility)GetValue(ItemsVisibleProperty); }
            set { SetValue(ItemsVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsVisibleProperty =
            DependencyProperty.Register("ItemsVisible", typeof(Visibility), typeof(InputExpander), new PropertyMetadata(Visibility.Collapsed));




        public bool IsExpanded {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsExpanded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(InputExpander), new PropertyMetadata(false, (d, s) => {
                if (d is InputExpander o)
                {
                    o.ActionIcon = o.IsExpanded ? "\uE70E" : "\uE70D";
                    o.HeaderCornerRadius = o.IsExpanded ? new CornerRadius(o.CornerRadius.TopLeft, o.CornerRadius.TopRight, 0, 0) : o.CornerRadius;
                    o.ItemsVisible = o.IsExpanded ? Visibility.Visible : Visibility.Collapsed;
                }
            }));

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is InputCard;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new InputCard
            {
                Margin = new Thickness(0),
                ActionIconVisible = Visibility.Collapsed,
                Background = new SolidColorBrush(Colors.Transparent),
                BorderThickness = new Thickness(0, 1, 0, 0),
                CornerRadius = new CornerRadius(0, 0, CornerRadius.BottomRight, CornerRadius.BottomLeft)
            };
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (item is InputCard o)
            {
                o.Margin = new Thickness(0);
                o.CornerRadius = Items.Count - 1 <= Items.IndexOf(item) 
                    ? new CornerRadius(0, 0, CornerRadius.BottomRight, CornerRadius.BottomLeft) : new CornerRadius(0);
                o.BorderThickness = new Thickness(0, 1, 0, 0);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (GetTemplateChild(HeaderBtnName) is  FrameworkElement o)
            {
                o.MouseDown += Header_MouseDown;
            }
        }

        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsExpanded = !IsExpanded;
        }
    }
}
