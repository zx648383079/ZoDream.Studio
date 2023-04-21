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
using ZoDream.Shared.Models;
using ZoDream.Shared.Utils;

namespace ZoDream.Studio.Controls
{
    public class PianoRollPanel : RollTablePanel
    {
        static PianoRollPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PianoRollPanel), new FrameworkPropertyMetadata(typeof(PianoRollPanel)));
        }

        private PianoPanel? PianoBar;

        protected override void HeaderLoadOverride(IRollHeaderBar? bar)
        {
            PianoBar = bar as PianoPanel;
        }

        protected override void MoveItemOverride(FrameworkElement item, double x, double y)
        {
            if (PianoBar != null && item is NoteBar o)
            {
                o.Label = PianoBar.Get(y).ToString();
            }
        }

        protected override FrameworkElement? GetContainerForItemOverride(double x, double y)
        {
            return new NoteBar()
            {
                Label = PianoBar!.Get(y).ToString()
            };
        }
    }
}
