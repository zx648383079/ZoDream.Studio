using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiPlayer.Midi
{
    public class Key
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
        /// 速度 .125 - 8  为0时为中止
        /// </summary>
        public float Speed { get; set; } = 1;

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

        public void SetKeyBy127(int code)
        {
            SetKeyBy12(code % 12 + 1);
            Beat = Convert.ToInt16(code / 12 - 4);
        }

        public byte ToKey127()
        {
            var code = (Beat + 4) * 12;
            if (Code < 3) {
                code += Code * 2 + Scale - 1;
            } else if (Code == 3) {
                code += 5;
            } else {
                code += 2 * Code - 2 + Scale;
            }
            return Convert.ToByte(code);
        }
        /// <summary>
        /// 获取白键的序号 0 开始
        /// </summary>
        /// <returns></returns>
        public int GetWhiteCount()
        {
            return (Beat + 4) * 7 + Code - 1;
        }
        
        public Tuple<byte, int> ToTuple(Score score)
        {
            var bar = Convert.ToInt32(60000 / score.Tempo * Speed);
            return new Tuple<byte, int>(ToKey127(), bar);
        }

        public Key() { }

        public Key(ushort code)
        {
            Code = code;
        }

        public Key(int code)
        {
            Code = Convert.ToUInt16(code);
        }

        public Key(ushort code, float speed)
        {
            Code = code;
            Speed = speed;
        }

        public Key(ushort code, byte scale, float speed)
        {
            Code = code;
            Scale = scale;
            Speed = speed;
        }

        public Key(ushort code, byte scale, short beat, float speed)
        {
            Code = code;
            Scale = scale;
            Speed = speed;
            Beat = beat;
        }

        public static Key Create127(int code)
        {
            var key = new Key();
            key.SetKeyBy127(code);
            return key;
        }
    }
}
