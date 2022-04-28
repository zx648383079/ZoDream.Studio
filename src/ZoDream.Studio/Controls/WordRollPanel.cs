﻿using System;
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
    ///     <MyNamespace:WordRollPanel/>
    ///
    /// </summary>
    [TemplatePart(Name = TrackPanel.TrackPanelName, Type = typeof(Canvas))]
    [TemplatePart(Name = TrackPanel.RuleName, Type = typeof(RulePanel))]
    [TemplatePart(Name = TrackPanel.HorizontalBarName, Type = typeof(ScrollBar))]
    [TemplatePart(Name = TrackPanel.VerticalBarName, Type = typeof(ScrollBar))]
    public class WordRollPanel : Control
    {
        static WordRollPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WordRollPanel), new FrameworkPropertyMetadata(typeof(WordRollPanel)));
        }

        private Canvas? BoxPanel;
        private RulePanel? Ruler;
        private ScrollBar? HorizontalBar;
        private ScrollBar? VerticalBar;
        private double HeaderWidth = 200.0;
        private double HorizontalOffset = .0;
        private double VerticalOffset = .0;
        private TrackBar? MoveBar;
        private Point MoveLast = new();

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BoxPanel = GetTemplateChild(TrackPanel.TrackPanelName) as Canvas;
            Ruler = GetTemplateChild(TrackPanel.RuleName) as RulePanel;
            HorizontalBar = GetTemplateChild(TrackPanel.HorizontalBarName) as ScrollBar;
            VerticalBar = GetTemplateChild(TrackPanel.VerticalBarName) as ScrollBar;
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
                VerticalBar.Maximum = 100;
                VerticalBar.ValueChanged += VerticalBar_ValueChanged;
            }
        }

        private void VerticalBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VerticalOffset = e.NewValue;
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
            HeaderWidth = BoxPanel!.ActualWidth - Ruler!.ActualWidth;
            UpdateSize();
        }

        private void UpdateSize()
        {

        }
    }
}
