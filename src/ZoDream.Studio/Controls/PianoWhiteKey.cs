﻿using System;
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
    ///     <MyNamespace:PianoWhiteKey/>
    ///
    /// </summary>
    public class PianoWhiteKey : Control
    {
        static PianoWhiteKey()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PianoWhiteKey), new FrameworkPropertyMetadata(typeof(PianoWhiteKey)));
        }

        public bool IsPressed
        {
            get { return (bool)GetValue(IsPressedProperty); }
            set { SetValue(IsPressedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPressed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPressedProperty =
            DependencyProperty.Register("IsPressed", typeof(bool), typeof(PianoWhiteKey), new PropertyMetadata(false));



        public PianoKey Value
        {
            get { return (PianoKey)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(PianoKey), typeof(PianoWhiteKey), new PropertyMetadata(PianoKey.Create127(60)));

        public bool Touchable {
            get { return (bool)GetValue(TouchableProperty); }
            set { SetValue(TouchableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Touchable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TouchableProperty =
            DependencyProperty.Register("Touchable", typeof(bool), typeof(PianoWhiteKey), new PropertyMetadata(true));



        public event PianoKeyEventHandler? OnPress;
        public event PianoKeyEventHandler? OnRelease;

        public void TapPress()
        {
            IsPressed = true;
            OnPress?.Invoke(this, new PianoKeyEventArgs(Value, true));
        }

        public void TapRelease()
        {
            IsPressed = false;
            OnRelease?.Invoke(this, new PianoKeyEventArgs(Value, false));
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (!Touchable)
            {
                return;
            }
            TapPress();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (!Touchable)
            {
                return;
            }
            TapRelease();
        }
    }
}
