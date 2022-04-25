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
    [TemplatePart(Name = "PART_PainoPanel", Type = typeof(Canvas))]
    public class PianoPanel : Control
    {
        static PianoPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PianoPanel), new FrameworkPropertyMetadata(typeof(PianoPanel)));
        }

        public int KeyWidth
        {
            get { return (int)GetValue(KeyWidthProperty); }
            set { SetValue(KeyWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for KeyWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyWidthProperty =
            DependencyProperty.Register("KeyWidth", typeof(int), typeof(PianoPanel), new PropertyMetadata(0));



        public int WhiteKeyWidth
        {
            get { return (int)GetValue(WhiteKeyWidthProperty); }
            set { SetValue(WhiteKeyWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WhiteKeyWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WhiteKeyWidthProperty =
            DependencyProperty.Register("WhiteKeyWidth", typeof(int), typeof(PianoPanel), new PropertyMetadata(0));



        public int BlackKeyWidth
        {
            get { return (int)GetValue(BlackKeyWidthProperty); }
            set { SetValue(BlackKeyWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BlackKeyWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BlackKeyWidthProperty =
            DependencyProperty.Register("BlackKeyWidth", typeof(int), typeof(PianoPanel), new PropertyMetadata(0));



        private Canvas? BoxPanel;
        private readonly PianoKey BeginKey = PianoKey.Create127(60);
        private byte OffsetKey = 60;

        public event PianoKeyEventHandler? OnPress;
        public event PianoKeyEventHandler? OnRelease;

        private double GetBlackKeyWidth()
        {
            if (BlackKeyWidth > 0)
            {
                return BlackKeyWidth;
            }
            if (KeyWidth > 0)
            {
                return KeyWidth * .6;
            }
            return GetWhiteKeyWidth() *.6;
        }

        private double GetWhiteKeyWidth()
        {
            if (WhiteKeyWidth > 0)
            {
                return WhiteKeyWidth;
            }
            if (KeyWidth > 0)
            {
                return KeyWidth;
            }
            return Math.Min(ActualWidth / 7, 50);
        }

        private double GetWhiteKeyHeight()
        {
            return ActualHeight;
        }

        private double GetBlackKeyHeight()
        {
            return GetWhiteKeyHeight() * .6;
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
            BoxPanel = GetTemplateChild("PART_PainoPanel") as Canvas;
            DrawKey();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateKey();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            OffsetKey = (byte)Math.Max(Math.Min(OffsetKey - Math.Floor(e.Delta / 60.0), 120), 0);
            UpdateKey(PianoKey.Create127(OffsetKey));
        }

        private void DrawKey()
        {
            DrawKey(BeginKey, 0);
        }

        private void DrawKey(PianoKey beginKey, double offset = 0)
        {
            DrawKey(offset - Get127Offset(beginKey));
        }

        private void DrawKey(double offset)
        {
            if (BoxPanel == null)
            {
                return;
            }
            BoxPanel.Children.Clear();
            for (int i = 0; i <= 127; i++)
            {
                var key = PianoKey.Create127(i);
                var x = Get127Offset(key) + offset;
                var y = .0;
                var w = key.Scale > 0 ? GetBlackKeyWidth() : GetWhiteKeyWidth();
                var h = key.Scale > 0 ? GetBlackKeyHeight() : GetWhiteKeyHeight();
                var keyBtn = key.Scale > 0 ? new PianoBlackKey() : new PianoWhiteKey();
                keyBtn.Value = key;
                keyBtn.Width = w;
                keyBtn.Height = h;
                Canvas.SetLeft(keyBtn, x);
                Canvas.SetTop(keyBtn, y);
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

        private double Get127Offset(PianoKey key)
        {
            return GetOffset(key);
        }

        private double Get88ffset(PianoKey key)
        {
            return GetOffset(key, PianoKey.Create127(21));
        }

        private double GetOffset(PianoKey key, PianoKey baseKey)
        {
            return GetOffset(key) - GetOffset(baseKey);
        }

        private double GetOffset(PianoKey key)
        {
            var bw = GetBlackKeyWidth();
            var ww = GetWhiteKeyWidth();
            return ((key.Beat + 4) * 7 + key.Code - 1) * ww + (key.Scale > 0 ? ww - bw / 2 : 0);
        }
        private void UpdateKey(PianoKey beginKey)
        {
            if (BoxPanel == null)
            {
                return;
            }
            var offset = - Get127Offset(beginKey);
            foreach (PianoWhiteKey item in BoxPanel.Children)
            {
                if (item == null)
                {
                    continue;
                }
                var key = item.Value;
                var x = Get127Offset(key) + offset;
                var y = .0;
                var w = key.Scale > 0 ? GetBlackKeyWidth() : GetWhiteKeyWidth();
                var h = key.Scale > 0 ? GetBlackKeyHeight() : GetWhiteKeyHeight();
                item.Width = w;
                item.Height = h;
                Canvas.SetLeft(item, x);
                Canvas.SetTop(item, y);
            }
        }

        private void UpdateKey()
        {
            UpdateKey(PianoKey.Create127(OffsetKey));
        }
    }

    public delegate void PianoKeyEventHandler(object sender, PianoKeyEventArgs e);

    public class PianoKeyEventArgs
    {
        public PianoKeyEventArgs(PianoKey key, bool isPressed)
        {
            Key = key;
            IsPressed = isPressed;
        }

        public PianoKey Key { get; private set; }

        public bool IsPressed { get; private set; }
        public bool Handle { get; set; } = false;
    }
}
