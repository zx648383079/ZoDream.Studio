using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class EncoderItem
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public EncoderItem(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
