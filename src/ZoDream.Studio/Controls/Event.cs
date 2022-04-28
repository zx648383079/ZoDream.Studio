using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Studio.Controls
{
    public enum MoveStatus
    {
        None,
        Move,
        SizeRight,
        SizeLeft,
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
