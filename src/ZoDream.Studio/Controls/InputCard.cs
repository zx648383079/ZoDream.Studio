using System;
using System.Collections.Generic;
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
    ///     <MyNamespace:InputCard/>
    ///
    /// </summary>
    public class InputCard : ButtonBase
    {
        static InputCard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(InputCard), new FrameworkPropertyMetadata(typeof(InputCard)));
        }



        public string Icon {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(InputCard), 
                new PropertyMetadata(string.Empty, (d, s) => {
                    if (d is InputCard box)
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
            DependencyProperty.Register("Header", typeof(string), typeof(InputCard), new PropertyMetadata(string.Empty));



        public string Description {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(InputCard), new PropertyMetadata(string.Empty, (d, s) => {
                if (d is InputCard box)
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
            DependencyProperty.Register("ActionIcon", typeof(string), typeof(InputCard), new PropertyMetadata("\uE76C", (d,s) => {
                if (d is InputCard box)
                {
                    box.ActionIconVisible = string.IsNullOrWhiteSpace(box.ActionIcon) ? Visibility.Collapsed : Visibility.Visible;
                }
            }));



        public Visibility ActionIconVisible {
            get { return (Visibility)GetValue(ActionIconVisibleProperty); }
            set { SetValue(ActionIconVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActionIconVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActionIconVisibleProperty =
            DependencyProperty.Register("ActionIconVisible", typeof(Visibility), typeof(InputCard), new PropertyMetadata(Visibility.Visible));



        public Visibility IconVisible {
            get { return (Visibility)GetValue(IconVisibleProperty); }
            set { SetValue(IconVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconVisibleProperty =
            DependencyProperty.Register("IconVisible", typeof(Visibility), typeof(InputCard), new PropertyMetadata(Visibility.Collapsed));




        public Visibility MetaVisible {
            get { return (Visibility)GetValue(MetaVisibleProperty); }
            set { SetValue(MetaVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MetaVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MetaVisibleProperty =
            DependencyProperty.Register("MetaVisible", typeof(Visibility), typeof(InputCard), new PropertyMetadata(Visibility.Collapsed));




        public CornerRadius CornerRadius {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(InputCard), new PropertyMetadata(null));


    }
}
