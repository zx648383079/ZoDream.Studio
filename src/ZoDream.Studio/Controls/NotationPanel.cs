using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// 简谱显示
    /// </summary>
    public class NotationPanel : Control
    {
        static NotationPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NotationPanel), new FrameworkPropertyMetadata(typeof(NotationPanel)));
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            DrawTitle(drawingContext);
        }


        private void DrawTitle(DrawingContext drawingContext)
        {
            var w = ActualWidth - Padding.Left - Padding.Right;
            var font = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
            var ft = new FormattedText(
                            "标题", CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                            font, FontSize * 1.5, Foreground, 1.25);
            drawingContext.DrawText(ft, new Point(Padding.Left + (w - ft.Width) / 2, 
                Padding.Top));

            ft = new FormattedText(
                            "** 词", CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                            font, FontSize, Foreground, 1.25);
            drawingContext.DrawText(ft, new Point(ActualWidth - Padding.Right - ft.Width ,
                Padding.Top));
            ft = new FormattedText(
                            "** 曲", CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                            font, FontSize, Foreground, 1.25);
            drawingContext.DrawText(ft, new Point(ActualWidth - Padding.Right - ft.Width,
                Padding.Top + FontSize));

            ft = new FormattedText(
                            "1 = G", CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                            font, FontSize, Foreground, 1.25);
            drawingContext.DrawText(ft, new Point(Padding.Left,
                Padding.Top + FontSize));
            DrawFraction(drawingContext, "1", "4", Padding.Left + ft.Width, FontSize / 2);

        }

        private void DrawFraction(DrawingContext drawingContext, string numerator, string denominator, double x, double y)
        {
            var font = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
            var nft = new FormattedText(
                            numerator, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                            font, FontSize, Foreground, 1.25);
            var dft = new FormattedText(
                            denominator, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                            font, FontSize, Foreground, 1.25);
            var w = Math.Max(nft.Width, dft.Width) + 10;
            drawingContext.DrawText(nft, new Point(x + (w - nft.Width) / 2, y));
            y += nft.Height + 2;
            drawingContext.DrawLine(new Pen(Foreground, 2), new Point(x, y), new Point(x+ w, y));
            y += 4;
            drawingContext.DrawText(dft, new Point(x + (w - dft.Width) / 2, y));
        }
    }
}
