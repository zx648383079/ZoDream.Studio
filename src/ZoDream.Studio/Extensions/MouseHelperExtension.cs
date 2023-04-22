using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZoDream.Shared.Utils;

namespace ZoDream.Studio.Extensions
{
    public static class MouseHelperExtension
    {

        public static void MouseDown(this MouseHelper helper, Point point)
        {
            helper.MouseDown(point.X, point.Y);
        }

        public static void MouseDown(this MouseHelper helper, int tag, Point point)
        {
            helper.MouseDown(tag, point.X, point.Y);
        }

        public static void MouseMove(this MouseHelper helper, Point point)
        {
            helper.MouseMove(point.X, point.Y);
        }

        public static void MouseUp(this MouseHelper helper, Point point)
        {
            helper.MouseUp(point.X, point.Y);
        }
    }
}
