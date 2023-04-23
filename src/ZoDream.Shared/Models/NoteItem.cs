using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    /// <summary>
    /// 一个音符
    /// </summary>
    public class NoteItem
    {

        public PianoKey Key { get; set; } = new PianoKey();

        public int Begin { get; set; }
        public int End { get; set; }

        public int Duration => End - Begin;
    }
}
