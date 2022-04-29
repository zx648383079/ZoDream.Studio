using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ZoDream.Shared.Models
{
    public class PianoKey
    {
        /// <summary>
        /// 调号 1 - 7
        /// </summary>
        public ushort Code { get; set; } = 0;

        /// <summary>
        /// 0 表示大调，1 表示小调
        /// </summary>
        public byte Scale { get; set; }

        /// <summary>
        /// 音调 -4 - 4
        /// </summary>
        public short Beat { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="code">1 - 12</param>
        public void SetKeyBy12(int code)
        {
            if (code == 5)
            {
                Code = 3;
                Scale = 0;
                return;
            }
            if (code < 6)
            {
                Code = Convert.ToUInt16((code + 1) / 2);
                Scale = Convert.ToByte(1 - code % 2);
                return;
            }
            Code = Convert.ToUInt16(code / 2 + 1);
            Scale = Convert.ToByte(code % 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code">0-127</param>
        public void SetKeyBy127(int code)
        {
            SetKeyBy12(code % 12 + 1);
            Beat = Convert.ToInt16(code / 12 - 4);
        }

        public void SetKey(string code)
        {
            var match = Regex.Match(code.Trim().ToUpper(), @"^([CDEFGAB])(#?)(\d+)$");
            if (match == null || !match.Success)
            {
                return;
            }
            Scale = Convert.ToByte(match.Groups[2].Value == "#" ? 1 : 0);
            Beat = Convert.ToInt16(Convert.ToInt16(match.Groups[3].Value) -4);
            Code = Convert.ToUInt16("CDEFGAB".IndexOf(match.Groups[1].Value) + 1);
        }

        public byte ToKey127()
        {
            var code = (Beat + 4) * 12;
            if (Code < 3)
            {
                code += Code * 2 + Scale - 1;
            }
            else if (Code == 3)
            {
                code += 5;
            }
            else
            {
                code += 2 * Code - 2 + Scale;
            }
            return Convert.ToByte(code - 1);
        }

        public string ToKey()
        {
            if (Code == 0)
            {
                return string.Empty;
            }
            var key = "CDEFGAB".Substring(Code - 1, 1);
            if (Scale > 0)
            {
                key += "#";
            }
            return $"{key}{Beat + 4}";
        }

        #region 拓展方法
        public override bool Equals(object? obj)
        {
            if (obj is null)
            {
                return false;
            }
            if (obj is PianoKey key)
            {
                return ToKey() == key.ToKey();
            }
            if (obj is string tag)
            {
                return ToKey() == tag;
            }
            if (obj is int code)
            {
                return ToKey127() == code;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return ToKey();
        }

        public static PianoKey operator -(PianoKey key, int offset)
        {
            return Create127(key.ToKey127() - offset);
        }

        public static PianoKey operator +(PianoKey key, int offset)
        {
            return Create127(key.ToKey127() + offset);
        }

        public static PianoKey operator -(PianoKey key, PianoKey offset)
        {
            return Create127(key.ToKey127() - offset.ToKey127());
        }

        public static PianoKey operator +(PianoKey key, PianoKey offset)
        {
            return Create127(key.ToKey127() + offset.ToKey127());
        }

        public static bool operator ==(PianoKey? key, object? con)
        {
            if (key is null)
            {
                return con is null;
            }
            return key.Equals(con);
        }

        public static bool operator !=(PianoKey? key, object? con)
        {
            if (key is null)
            {
                return con is not null;
            }
            return !key.Equals(con);
        }

        #endregion


        public static PianoKey Create127(int code)
        {
            var key = new PianoKey();
            key.SetKeyBy127(code);
            return key;
        }

        public static PianoKey Create(string code)
        {
            var key = new PianoKey();
            key.SetKey(code);
            return key;
        }

        public static PianoKey Create(object? key)
        {
            if (key is null)
            {
                return Create127(0);
            }
            if (key is PianoKey k)
            {
                return k;
            }
            var s = key.ToString();
            if (s == null)
            {
                return Create127(0);
            }
            if (Regex.IsMatch(s, @"^([CDEFGAB])(#?)(\d+)$"))
            {
                return Create(s!);
            }
            return Create127(Convert.ToInt32(s));
        }
    }
}
