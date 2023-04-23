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
using ZoDream.Shared.Models;
using ZoDream.Shared.ViewModel;

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
    ///     <MyNamespace:TrackHeader/>
    ///
    /// </summary>
    public class TrackHeader : Control
    {
        static TrackHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TrackHeader), new FrameworkPropertyMetadata(typeof(TrackHeader)));
        }

        public TrackHeader()
        {
            ActionCommand = new RelayCommand(o => {
                Command?.Execute(new TrackActionEventArgs(Data, o is string i && i == "1"));
            });
        }

        public string Header {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(TrackHeader), new PropertyMetadata(string.Empty));



        public bool IsLocked {
            get { return (bool)GetValue(IsLockedProperty); }
            set { SetValue(IsLockedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLocked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLockedProperty =
            DependencyProperty.Register("IsLocked", typeof(bool), typeof(TrackHeader), new PropertyMetadata(false, (d, s) => {
                (d as TrackHeader)?.SyncData();
            }));




        public bool IsHidden {
            get { return (bool)GetValue(IsHiddenProperty); }
            set { SetValue(IsHiddenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsHidden.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsHiddenProperty =
            DependencyProperty.Register("IsHidden", typeof(bool), typeof(TrackHeader), new PropertyMetadata(false));




        public ICommand Command {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(TrackHeader), new PropertyMetadata(null));



        public ICommand ActionCommand {
            get { return (ICommand)GetValue(ActionCommandProperty); }
            set { SetValue(ActionCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActionCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActionCommandProperty =
            DependencyProperty.Register("ActionCommand", typeof(ICommand), typeof(TrackHeader), new PropertyMetadata(null));




        private ProjectTrackItem data;

        public ProjectTrackItem Data {
            get { return data; }
            set { 
                data = value;
                IsHidden = value.IsHidden;
                IsLocked = value.IsLocked;
                Header = value.Name;
            }
        }



        public int RowIndex { get; set; }


        private void SyncData()
        {
            if (Data is null)
            {
                return;
            }
            Data.IsLocked = IsLocked;
            Data.IsHidden = IsHidden;
        }
    }
}
