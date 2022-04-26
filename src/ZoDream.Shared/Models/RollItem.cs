using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    /// <summary>
    /// 一卷
    /// </summary>
    public class RollItem
    {
        public int Offset { get; set; }

        public int Begin { get; set; }

        public int End { get; set; }

        public NoteItem Data { get; set; }

        public RollItem(NoteItem data)
        {
            Data = data;
        }
    }
}
