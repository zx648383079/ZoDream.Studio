using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Utils
{
    public static class ByteHelper
    {

        #region 辅助方法
        /// <summary>
        /// 生成可变字节
        /// </summary>
        /// <param name="val">输入值</param>
        /// <returns>基于 Big-Endian 的字节数组</returns>
        public static byte[] ConvertToVariableLength(uint val)
        {
            if (val == 0)
            {
                return new byte[] { 0 };
            }
            var tempvalues = new List<uint>();
            uint tempval = val;
            do
            {
                /*
                 * 从右往左，每7个二进制位组合
                 * 比如有两个字节
                 * 0001 1100    1111 1011
                 * 总共十六位，去掉左边两位，变成14位，再组合起来
                 * 0111001 1111011
                 * 0x7F = 0111 1111，去掉第7位以后的数字
                 */
                tempvalues.Add(tempval & 0x7F);
            }
            while ((tempval >>= 7) > 0);// 每次移动7位，正好形成每个字节的前7位（第8位是标志位）

            // 每7位形成一个字节后
            // 第一个字节的第8位为0，表示可变长度的结束位
            // 其它字节的第8位必须为1（0x80 = 1000 0000）
            // 表示可变长度的非结束位
            // 与 0x80 组合后正好可以将第8位变为1，而其他位不会丢失
            if (tempvalues.Count > 1)
            {
                for (var k = 1; k < tempvalues.Count; k++)
                {
                    tempvalues[k] |= 0x80;
                }
            }

            // 反转顺序，必须，因为存放到MIDI文件中的顺序是正好相反的
            tempvalues.Reverse();

            var data = new byte[tempvalues.Count];
            for (var i = 0; i < tempvalues.Count; i++)
            {
                data[i] = Convert.ToByte(tempvalues[i]);
            }

            return data;
        }

        /// <summary>
        /// 从可变长度字节数组中读出实际长度
        /// </summary>
        public static uint ConvertFromVariableLength(byte[] buffer)
        {
            // 读出来的是 Big Endian Bytes
            // 如果当前CPU是以 Little Endian 顺序存储
            // 就必须反转字节顺序
            if (buffer.Length == 1 && buffer[0] == 0x00)
            {
                return 0;
            }

            Array.Reverse(buffer);//反转

            var result = 0u;
            // 有效位是前7位，即 0111 1111
            // 第8位是标志位，无需处理
            var validates = 0x7Fu;
            /*
             假设有三个字节，分别是
             0111 1001
             0101 1111
             0000 0011
             各取前7位，组成结果
              11 101 1111 111 1001

            第一个字节向左移0位，第二个字节向左移7位，第三个字节向左移14位……
            即
              0000 0000  0000 0000  0000 0000  0111 1111
              0000 0000  0000 0000  0011 1111  1000 0000
              0000 0000  0001 1111  1100 0000  0000 0000
              0000 1111  1110 0000  0000 0000  0000 0000
            然后组合起来就是一个整数值了
             */
            for (var i = 0; i < buffer.Length; i++)
            {
                var temp = buffer[i] & validates;
                temp <<= (7 * i);
                result |= temp;
            }
            return result;
        }

        /// <summary>
        /// 将16位整数转为 Big Endian 字节顺序
        /// </summary>
        public static byte[] Convert16BitnumToBEBytes(ushort val)
        {
            if (val == 0)
            {
                return new byte[] { 0, 0 };
            }
            uint temp = val;
            List<byte> parts = new List<byte>();
            do
            {
                var b = (byte)(temp & 0xFF);
                parts.Add(b);
            }
            while ((temp >>= 8) > 0);

            var srcArr = parts.ToArray();
            var newArr = new byte[2];
            srcArr.CopyTo(newArr, 0);
            Array.Reverse(newArr);
            return newArr;
        }

        /// <summary>
        /// 从字节中读出16位整数
        /// </summary>
        public static ushort Convert16BitFromBEBytes(byte[] buffer)
        {
            if (buffer.Length != 2)
            {
                throw new ArgumentException("16位数值只允许两个字节");
            }
            ushort result = 0;
            // 反转
            Array.Reverse(buffer);
            for (int i = 0; i < buffer.Length; i++)
            {
                result |= (ushort)(buffer[i] << (8 * i));
            }
            return result;
        }

        /// <summary>
        /// 将 32 位整数转为 Big endian 顺序的字节
        /// </summary>
        public static byte[] Convert32BitnumToBEBytes(uint val)
        {
            if (val == 0)
            {
                return new byte[] { 0, 0, 0, 0 };
            }
            uint temp = val;
            var parts = new List<byte>();
            do
            {
                var tb = Convert.ToByte(temp & 0xFF);
                parts.Add(tb);
            }
            while ((temp >>= 8) > 0);
            // 32位整数是4个字节，但是转化后可能不足4个字节
            var src = parts.ToArray();
            var newbuffer = new byte[4];
            src.CopyTo(newbuffer, 0);
            // 反转
            Array.Reverse(newbuffer);
            return newbuffer;
        }

        /// <summary>
        /// 读出32位整数
        /// </summary>
        public static uint Convert32BitFromBEBytes(byte[] buffer)
        {
            if (buffer.Length != 4)
            {
                throw new ArgumentException("32位整数必须是四个字节");
            }
            var res = 0u;
            // 反转
            Array.Reverse(buffer);
            for (var n = 0; n < buffer.Length; n++)
            {
                res |= (uint)(buffer[n] << (8 * n));
            }
            return res;
        }

        /// <summary>
        /// 将整数值转为 24 位 big endian 顺序的字节，这个用于设置节拍元数据
        /// </summary>
        public static byte[] Convert24BitnumToBEBytes(uint val)
        {
            /*
             * 0000 0000  0000 0000  1111 1111
             * 0000 0000  1111 1111  0000 0000
             * 1111 1111  0000 0000  0000 0000
             */
            var data = new List<byte>();
            for (int i = 0; i < 3; i++)
            {
                var temp = val >> (i * 8);
                temp &= 0xFF;
                data.Add((byte)temp);
            }
            data.Reverse();
            return data.ToArray();
        }

        /// <summary>
        /// 从字节数组中读出24位数值
        /// </summary>
        public static uint Convert24BitFromBEBytes(byte[] buffer)
        {
            if (buffer.Length != 3)
            {
                throw new ArgumentException("24位整数必须为三个字节");
            }
            Array.Reverse(buffer); //反转
            var res = 0u;
            for (var i = 0; i < buffer.Length; i++)
            {
                uint t = buffer[i];
                res |= t << (i * 8);
            }
            return res;
        }

        /// <summary>
        /// 用单个字节存放有符号整数。主要用于转换调号
        /// </summary>
        public static byte ConvertSignedValToSinglebyte(short val)
        {
            // 负数在计算机中是以 取反 + 1（补码）的形式存储
            /*
             * 比如-7，+7的表示如下
             * 0000 0000  0000 0111
             * 要变成-7，首先要取反，变成
             * 1111 1111  1111 1000
             * 然后加1（补码），得到
             * 1111 1111  1111 1001
             * 可是这里头有两个字节，若能存一个字节，那只好把高字节去掉，只要跟0xFF做【与】运算就能去掉了。
             * 
             *  1111 1111  1111 1001
             * -------- And ----------
             *  0000 0000  1111 1111
             *  ------- 结果 ---------
             *  0000 0000  1111 1001
             */
            var r = (short)(val & 0xFF);
            return (byte)r;
        }
        /// <summary>
        /// 从单个字节中读出带符号的整数
        /// </summary>
        public static short ConvertSignedValFromSinglebyte(byte bt)
        {
            short result;
            // 如果第8位为0，表示大于/等于0的数，如果为1则为负值
            if ((bt & 0x80) == 0x80)
            {
                // 负值 1111 1111  1xxx xxxx
                result = (short)(0xFF00 | bt);
            }
            else
            {
                result = bt;
            }
            return result;
        }
        #endregion


    }
}
