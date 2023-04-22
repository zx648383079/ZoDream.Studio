using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ZoDream.Studio.Controls
{
    public class WordRollPanel : RollTablePanel
    {
        static WordRollPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WordRollPanel), new FrameworkPropertyMetadata(typeof(WordRollPanel)));
        }


        private VolumeBar? VolumeBar;

        protected override void HeaderLoadOverride(IRollHeaderBar? bar)
        {
            VolumeBar = bar as VolumeBar;
        }

        protected override void MoveItemOverride(FrameworkElement item, double x, double y)
        {
            
        }

        protected override FrameworkElement? GetContainerForItemOverride(double x, double y)
        {
            return null;
        }
    }
}
