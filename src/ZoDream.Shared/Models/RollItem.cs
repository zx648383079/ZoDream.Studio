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

        public List<NoteItem> Items { get; set; } = new();
    }
}
