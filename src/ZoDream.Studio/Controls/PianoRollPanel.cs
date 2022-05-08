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
    ///     <MyNamespace:PianoRollPanel/>
    ///
    /// </summary>
    [TemplatePart(Name = TrackPanel.TrackPanelName, Type = typeof(Canvas))]
    [TemplatePart(Name = TrackPanel.RuleName, Type = typeof(RulePanel))]
    [TemplatePart(Name = TrackPanel.HorizontalBarName, Type = typeof(ScrollBar))]
    [TemplatePart(Name = TrackPanel.VerticalBarName, Type = typeof(ScrollBar))]
    [TemplatePart(Name = PianoName, Type = typeof(PianoPanel))]
    public class PianoRollPanel : Control
    {
        public const string PianoName = "PART_Piano";
        static PianoRollPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PianoRollPanel), new FrameworkPropertyMetadata(typeof(PianoRollPanel)));
        }

        private PianoPanel? Piano;
        private Canvas? BoxPanel;
        private RulePanel? Ruler;
        private ScrollBar? HorizontalBar;
        private ScrollBar? VerticalBar;
        private double HorizontalOffset = .0;
        private double VerticalOffset = .0;
        private NoteBar? MoveBar;
        private Point MoveLast = new();
        private MoveStatus MoveStatus = MoveStatus.None;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BoxPanel = GetTemplateChild(TrackPanel.TrackPanelName) as Canvas;
            Ruler = GetTemplateChild(TrackPanel.RuleName) as RulePanel;
            HorizontalBar = GetTemplateChild(TrackPanel.HorizontalBarName) as ScrollBar;
            VerticalBar = GetTemplateChild(TrackPanel.VerticalBarName) as ScrollBar;
            Piano = GetTemplateChild(PianoName) as PianoPanel;
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
                VerticalBar.Maximum = 132;
                VerticalBar.ValueChanged += VerticalBar_ValueChanged;
            }
            if (BoxPanel != null)
            {
                BoxPanel.MouseEnter += BoxPanel_MouseEnter;
                BoxPanel.MouseLeave += BoxPanel_MouseLeave;
                BoxPanel.MouseLeftButtonDown += BoxPanel_MouseLeftButtonDown;
                BoxPanel.MouseLeftButtonUp += BoxPanel_MouseLeftButtonUp;
                BoxPanel.MouseMove += BoxPanel_MouseMove;
            }
            if (Piano != null)
            {
                Piano.OnScroll += Piano_OnScroll;
            }
        }

        private void Piano_OnScroll(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VerticalOffset = e.NewValue;
            VerticalBar!.Value = e.NewValue / 30;
            UpdateSize();
        }

        private void BoxPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (MoveBar == null || e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }
            var x = Canvas.GetLeft(MoveBar);
            var p1 = e.GetPosition(this);
            var diffX = p1.X - MoveLast.X;
            switch (MoveStatus)
            {
                case MoveStatus.Move:
                    var y = Canvas.GetTop(MoveBar);
                    Canvas.SetLeft(MoveBar, diffX + x);
                    Canvas.SetTop(MoveBar, p1.Y - MoveLast.Y + y);
                    break;
                case MoveStatus.SizeRight:
                    MoveBar.Width = MoveBar.ActualWidth + diffX;
                    break;
                case MoveStatus.SizeLeft:
                    MoveBar.Width = MoveBar.ActualWidth - diffX;
                    Canvas.SetLeft(MoveBar, x + diffX);
                    break;
                default:
                    break;
            }
            MoveLast = p1;
        }

        private void BoxPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (MoveBar != null && MoveStatus == MoveStatus.Move)
            {
                var y = Math.Floor((Canvas.GetTop(MoveBar) + VerticalOffset) / 30) * 30;
                y -= VerticalOffset;
                MoveBar.Label = Piano!.Get(y).ToString();
                Canvas.SetTop(MoveBar, y);
            }
            MoveBar = null;
        }

        private void BoxPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MoveBar != null)
            {
                return;
            }
            Add(e.GetPosition(BoxPanel));
        }

        private void BoxPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        private void BoxPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Pen;
        }

        private void VerticalBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VerticalOffset = Math.Min(e.NewValue * 30, 134 * 30 - ActualHeight);
            Piano!.Offset = VerticalOffset;
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
            // UpdateSize();
        }

        private void UpdateSize()
        {

        }

        private void Add(Point point)
        {
            if (BoxPanel == null)
            {
                return;
            }
            var y = Math.Floor((point.Y + VerticalOffset) / 30) * 30;
            var bar = new NoteBar
            {
                Height = 30,
                Width = 100
            };
            y -= VerticalOffset;
            bar.Label = Piano!.Get(y).ToString();
            Canvas.SetLeft(bar, point.X);
            Canvas.SetTop(bar, y);
            
            BoxPanel.Children.Add(bar);
            bar.MouseMove += Bar_MouseMove;
            bar.MouseLeftButtonDown += Bar_MouseLeftButtonDown;
            bar.MouseRightButtonDown += Bar_MouseRightButtonDown;
            bar.MouseLeave += Bar_MouseLeave;
        }

        private void Bar_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Pen;
        }

        private void Bar_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            BoxPanel!.Children.Remove((NoteBar)sender);
        }

        private void Bar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MoveBar = (NoteBar)sender;
            MoveLast = e.GetPosition(this);
        }

        private void Bar_MouseMove(object sender, MouseEventArgs e)
        {
            if (MoveBar != null)
            {
                return;
            }
            var bar = (NoteBar)sender;
            var p = e.GetPosition(bar);
            if (p.X < 5)
            {
                Cursor = Cursors.SizeWE;
                MoveStatus = MoveStatus.SizeLeft;
            } else if (p.X > bar.ActualWidth - 5)
            {
                Cursor = Cursors.SizeWE;
                MoveStatus = MoveStatus.SizeRight;
            }
            else
            {
                Cursor = Cursors.SizeAll;
                MoveStatus = MoveStatus.Move;
            }
        }
    }
}
