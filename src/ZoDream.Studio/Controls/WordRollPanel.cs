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
        private double RowHeight = 30.0;
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
            if (BoxPanel != null)
            {
                InitVolumeLine();
            }
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
                VerticalBar.Maximum = 127;
                VerticalBar.ValueChanged += VerticalBar_ValueChanged;
            }
        }

        private void VerticalBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VerticalOffset = Math.Min(e.NewValue * RowHeight, 130 * RowHeight - ActualHeight);
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

        public void InitVolumeLine()
        {
            for (int i = 127; i >= 0; i--)
            {
                var item = new Label()
                {
                    Content = i,
                    Height = RowHeight,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center
                };
                UpdateRow(item);
                BoxPanel!.Children.Add(item);
            }
        }

        private void UpdateSize()
        {
            if (BoxPanel == null)
            {
                return;
            }
            foreach (var item in BoxPanel.Children)
            {
                if (item == null)
                {
                    continue;
                }
                if (item is Label header)
                {
                    UpdateRow(header);
                    continue;
                }
                if (item is NoteBar row)
                {
                    UpdateRow(row);
                }
            }
        }

        private void UpdateRow(Label header)
        {
            header.Width = HeaderWidth;
            Canvas.SetTop(header, (127 - (int)header.Content) * RowHeight - VerticalOffset);
        }

        private void UpdateRow(NoteBar row)
        {
            Canvas.SetLeft(row, HeaderWidth - HorizontalOffset);
            // Canvas.SetTop(row, row.RowIndex * RowHeight - VerticalOffset);
        }
    }
}
