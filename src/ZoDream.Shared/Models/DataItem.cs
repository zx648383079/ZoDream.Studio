using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class DataItem
    {
        public string Name { get; set; } = string.Empty;

        public object? Value { get; set; }

        public DataItem()
        {
            
        }

        public DataItem(string name)
        {
            Name = name;
        }

        public DataItem(string name, object arg): this(name)
        {
            Value = arg;
        }
    }
}
