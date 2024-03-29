﻿using System;
using System.Collections.Generic;
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
using ZoDream.Shared.Models;
using ZoDream.Shared.Utils;

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
    ///     <MyNamespace:PianoPanel/>
    ///
    /// </summary>
    [TemplatePart(Name = PianoPanelName, Type = typeof(Canvas))]
    public class PianoKeyboardPanel : Control, IRollHeaderBar
    {
        public const string PianoPanelName = "PART_PianoPanel";
        static PianoKeyboardPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PianoKeyboardPanel), new FrameworkPropertyMetadata(typeof(PianoKeyboardPanel)));
        }


        public int WhiteKeyWidth
        {
            get { return (int)GetValue(WhiteKeyWidthProperty); }
            set { SetValue(WhiteKeyWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WhiteKeyWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WhiteKeyWidthProperty =
            DependencyProperty.Register("WhiteKeyWidth", typeof(int), typeof(PianoKeyboardPanel), new PropertyMetadata(0));



        public int BlackKeyWidth
        {
            get { return (int)GetValue(BlackKeyWidthProperty); }
            set { SetValue(BlackKeyWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BlackKeyWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BlackKeyWidthProperty =
            DependencyProperty.Register("BlackKeyWidth", typeof(int), typeof(PianoKeyboardPanel), new PropertyMetadata(0));



        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(PianoKeyboardPanel), new PropertyMetadata(Orientation.Horizontal));




        public object BeginKey
        {
            get { return GetValue(BeginKeyProperty); }
            set { SetValue(BeginKeyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BeginKey.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BeginKeyProperty =
            DependencyProperty.Register("BeginKey", typeof(object), typeof(PianoKeyboardPanel), new PropertyMetadata(60, (d, e) =>
            {
                (d as PianoKeyboardPanel)?.UpdateBeginKey();
            }));



        public double Offset
        {
            get { return (double)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Offset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(double), typeof(PianoKeyboardPanel), new PropertyMetadata(.0, (d, e) =>
            {
                (d as PianoKeyboardPanel)?.UpdateKey();
            }));




        public bool IsSameGap
        {
            get { return (bool)GetValue(IsSameGapProperty); }
            set { SetValue(IsSameGapProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSameGap.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSameGapProperty =
            DependencyProperty.Register("IsSameGap", typeof(bool), typeof(PianoKeyboardPanel), new PropertyMetadata(false));




        public object Min
        {
            get { return GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Min.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof(object), typeof(PianoKeyboardPanel), new PropertyMetadata(0, (d, e) =>
            {
                (d as PianoKeyboardPanel)?.DrawKey();
            }));




        public object Max
        {
            get { return GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Max.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(object), typeof(PianoKeyboardPanel), new PropertyMetadata(131, (d, e) =>
            {
                (d as PianoKeyboardPanel)?.DrawKey();
            }));



        public bool Touchable {
            get { return (bool)GetValue(TouchableProperty); }
            set { SetValue(TouchableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Touchable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TouchableProperty =
            DependencyProperty.Register("Touchable", typeof(bool), typeof(PianoKeyboardPanel), new PropertyMetadata(true));



        private Canvas? BoxPanel;
        private bool IsLazy = true;
        private readonly PianoDraw PianoHelper = new();
        public readonly PianoKey DefaultBeginKey = PianoKey.Create127(60);

        public event RoutedPropertyChangedEventHandler<double>? OnScroll;
        public event PianoKeyEventHandler? OnPress;
        public event PianoKeyEventHandler? OnRelease;

        private void UpdateBeginKey()
        {
            if (GetWhiteKeyWidth() <= 0)
            {
                IsLazy = true;
                return;
            }
            SyncSize();
            PianoHelper.Begin(GetBeginKey());
            Offset = PianoHelper.Offset;
            OnScroll?.Invoke(this, new RoutedPropertyChangedEventArgs<double>(0, Offset));
        }

        private PianoKey GetBeginKey()
        {
            if (BeginKey is PianoKey b)
            {
                return b;
            }
            var k = PianoKey.Create(BeginKey);
            SetCurrentValue(BeginKeyProperty, k);
            return k;
        }

        private PianoKey GetMin()
        {
            var k = PianoKey.Create(Min);
            k.Scale = 0;
            if (Min == k)
            {
                return k;
            }
            SetCurrentValue(MinProperty, k);
            return k;
        }

        private PianoKey GetMax()
        {
            var k = PianoKey.Create(Max);
            if (k.Scale > 0)
            {
                k += 1;
            }
            if (Max == k)
            {
                return k;
            }
            SetCurrentValue(MaxProperty, k);
            return k;
        }



        private double GetBlackKeyWidth()
        {
            if (BlackKeyWidth > 0)
            {
                return BlackKeyWidth;
            }
            return GetWhiteKeyWidth() *.6;
        }

        private double GetWhiteKeyWidth()
        {
            if (WhiteKeyWidth > 0)
            {
                return WhiteKeyWidth;
            }
            if (BlackKeyWidth > 0)
            {
                return BlackKeyWidth * 2;
            }
            return Math.Max(
                Math.Min((Orientation == Orientation.Horizontal ? ActualWidth : ActualHeight) / 7, 
                50), 30);
        }

        private double GetWhiteKeyHeight()
        {
            return Orientation == Orientation.Horizontal ? ActualHeight : ActualWidth;
        }

        private double GetBlackKeyHeight()
        {
            return GetWhiteKeyHeight() * .618;
        }

        private int PreColumnCount()
        {
            return Convert.ToInt32(Math.Min((Orientation == Orientation.Horizontal ? ActualWidth : ActualHeight) / GetWhiteKeyWidth() * 1.5, 4));
        }

        public PianoKey Get(double offset, bool isRelative = true)
        {
            return PianoHelper.Get(offset, isRelative);
        }

        public double ToOffset(PianoKey key)
        {
            return PianoHelper.GetOffsetToMax(key);
        }
        public double GetKeyPosition(PianoKey key)
        {
            return PianoHelper!.GetKeyPosition(key);
        }

        public void Press(PianoKey key)
        {
            TapKey(key, true);
        }

        public void Release(PianoKey key)
        {
            TapKey(key, false);
        }

        private void TapKey(PianoKey key, bool isPressed)
        {
            var item = GetKey(key);
            if (item == null)
            {
                if (isPressed)
                {
                    OnPress?.Invoke(this, new PianoKeyEventArgs(key, isPressed));
                } else
                {
                    OnRelease?.Invoke(this, new PianoKeyEventArgs(key, isPressed));
                }
                return;
            }
            if (isPressed)
            {
                item.TapPress();
            } else
            {
                item.TapRelease();
            }
        }

        private PianoWhiteKey? GetKey(PianoKey key)
        {
            if (BoxPanel == null)
            {
                return null;
            }
            foreach (var item in BoxPanel.Children)
            {
                if (item is PianoWhiteKey ob && ob.Value == key)
                {
                    return ob;
                }
            }
            return null;
        }

        public async Task TapKeyAsync(PianoKey key, int time)
        {
            TapKey(key, true);
            await Task.Delay(time);
            TapKey(key, false);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BoxPanel = GetTemplateChild(PianoPanelName) as Canvas;
            DrawKey();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (IsLazy)
            {
                PianoHelper.Begin(GetBeginKey());
                SetCurrentValue(OffsetProperty, PianoHelper.Offset);
                OnScroll?.Invoke(this, new RoutedPropertyChangedEventArgs<double>(0, Offset));
                IsLazy = false;
            }
            UpdateKey();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            var offset = PianoHelper.Move(-e.Delta);
            var args = new RoutedPropertyChangedEventArgs<double>(0, PianoHelper.Offset);
            OnScroll?.Invoke(this, args);
            if (args.Handled == true)
            {
                return;
            }
            Offset = PianoHelper.Offset;
        }

        private void SyncSize()
        {
            PianoHelper.Offset = Offset;
            PianoHelper.Max = GetMax();
            PianoHelper.Min = GetMin();
            PianoHelper.BoxWidth = ActualWidth;
            PianoHelper.BoxHeight = ActualHeight;
            PianoHelper.BlackKeyHeight = GetBlackKeyHeight();
            PianoHelper.BlackKeyWidth = GetBlackKeyWidth();
            PianoHelper.WhiteKeyWidth = GetWhiteKeyWidth();
            PianoHelper.WhiteKeyHeight = GetWhiteKeyHeight();
            PianoHelper.IsHorizontal = Orientation == Orientation.Horizontal;
            PianoHelper.IsSameGap = IsSameGap;
        }

        private void DrawKey()
        {
            if (BoxPanel == null)
            {
                return;
            }
            SyncSize();
            BoxPanel.Children.Clear();
            //var binding = new Binding
            //{
            //    Source = this,
            //    Mode = BindingMode.OneWay,
            //    Path = new PropertyPath("Touchable")
            //};
            for (int i = GetMin().ToKey127(); i <= GetMax().ToKey127(); i++)
            {
                var key = PianoKey.Create127(i);
                var keyBtn = key.Scale > 0 ? new PianoBlackKey() : new PianoWhiteKey();
                keyBtn.Value = key;
                // keyBtn.SetBinding(TouchableProperty, binding);
                keyBtn.Touchable = Touchable;
                UpdateKeyRect(keyBtn);
                Panel.SetZIndex(keyBtn, key.Scale * 55);
                BoxPanel.Children.Add(keyBtn);
                keyBtn.OnPress += (_, k) =>
                {
                    OnPress?.Invoke(this, k);
                };
                keyBtn.OnRelease += (_, k) =>
                {
                    OnRelease?.Invoke(this, k);
                };
            }
        }

        private void UpdateKeyRect(PianoWhiteKey item)
        {
            if (ActualWidth <= 0 || ActualHeight <= 0)
            {
                return;
            }
            
            if (Orientation == Orientation.Horizontal)
            {
                item.HorizontalContentAlignment = HorizontalAlignment.Center;
                item.VerticalContentAlignment = VerticalAlignment.Bottom;
                
            } else
            {
                item.HorizontalContentAlignment = HorizontalAlignment.Right;
                item.VerticalContentAlignment = VerticalAlignment.Center;
            }
            var rect = PianoHelper.Invalidate(item.Value);
            item.Width = rect[2];
            item.Height = rect[3];
            Canvas.SetLeft(item, rect[0]);
            Canvas.SetTop(item, rect[1]);
        }


        private void UpdateKey()
        {
            if (BoxPanel == null)
            {
                return;
            }
            SyncSize();
            foreach (PianoWhiteKey item in BoxPanel.Children)
            {
                if (item == null)
                {
                    continue;
                }
                UpdateKeyRect(item);
            }
        }

        
    }

    
}
