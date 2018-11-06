using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiPlayer.Midi
{
    public class Drawer
    {
        public Player Driver { get; set; }

        public Cancas Box { get; set; }

        public IList<IButton> KeyList { get; set; } = new List<Button>();

        public void Refresh()
        {

        }

        public void Tap(byte key, int time)
        {
            Press(key, time);

        }

        public void Tap(Tuple tuple)
        {
            Tap(tuple.Item1, tuple.Item2);
        }

        public void Press(byte key, int time)
        {

        }
        public void Press(Tuple tuple)
        {
            Press(tuple.Item1, tuple.Item2);
        }

        public void PressDown(byte key)
        {

        }
        
        public void PressUp(byte key)
        {

        }

    }
}
