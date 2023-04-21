using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ZoDream.Studio.Controls
{
    public interface IRollHeaderBar
    {

        public double Offset { get; set; }
        public event RoutedPropertyChangedEventHandler<double>? OnScroll;
    }
}
