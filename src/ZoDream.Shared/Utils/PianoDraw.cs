using System;
using System.Collections.Generic;
using System.Text;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Utils
{
    public class PianoDraw
    {
        public double BoxWidth { get; set; }

        public double BoxHeight { get; set; }

        public bool IsHorizontal { get; set; } = true;

        public bool IsVertical { 
            get
            {
                return !IsHorizontal;
            }
            set
            {
                IsHorizontal = !value;
            }
        }
        /// <summary>
        /// 全部为正数，为距离top 的距离
        /// </summary>
        public double Offset { get; set; }

        private double whiteKeyWidth;

        public double WhiteKeyWidth
        {
            get { return whiteKeyWidth; }
            set { whiteKeyWidth = value; IsInvalidated = false; }
        }

        private double blackKeyWidth;

        public double BlackKeyWidth
        {
            get { return blackKeyWidth; }
            set { blackKeyWidth = value; IsInvalidated = false; }
        }

        public double BlackKeyHeight { get; set; }

        public double WhiteKeyHeight { get; set; }
        /// <summary>
        /// 是否是等间距以黑键为基准，白键自适应
        /// </summary>
        public bool IsSameGap { get; set; } = false;

        /// <summary>
        ///  88 键是从 21开始
        /// </summary>
        public PianoKey Min { get; set; } = PianoKey.Create127(0);
        public PianoKey Max { get; set; } = PianoKey.Create127(131);
        private double MinOffset;
        private double MaxOffset;
        private double MaxToMin;
        /// <summary>
        /// 总长度
        /// </summary>
        private double BarLength;
        private bool IsInvalidated = false;

        public void Begin(PianoKey key)
        {
            if (IsVertical)
            {
                Offset = BarLength - GetOffsetFromZero(key) - BoxHeight;
                return;
            }
            Offset = GetOffsetFromMin(key);
        }

        /// <summary>
        /// 移动距离
        /// </summary>
        /// <param name="offset">负数往前</param>
        /// <returns>真实移动距离</returns>
        public double Move(double offset)
        {
            Invalidate();
            var val = Math.Min(Math.Max(0, Offset + offset), BarLength -
                (IsHorizontal ? BoxWidth : BoxHeight));
            var diff = val - val;
            Offset = val;
            return diff;
        }

        /// <summary>
        /// 这是获取当前位置
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="isRelative">是否是相对坐标</param>
        /// <returns></returns>
        public PianoKey Get(double offset, bool isRelative = true)
        {
            Invalidate();
            if (isRelative)
            {
                offset += Offset;
            }
            if (IsVertical)
            {
                offset = BarLength - offset;
            }
            if (IsSameGap)
            {
                return PianoKey.Create127(Convert.ToInt32(offset / BlackKeyWidth) 
                    - (IsVertical ? 1 : 0));
            }
            var w = WhiteKeyWidth;
            var index = offset / w;
            var key = new PianoKey
            {
                Beat = Convert.ToInt16(index / 7 - 4),
                Code = Convert.ToUInt16(index % 7 + 1)
            };
            if (key.ToKey127() > 0)
            {
                var pre = key - 1;
                if (pre.Scale > 0 && GetOffsetFromMin(pre) + BlackKeyWidth > offset)
                {
                    return pre;
                }
            }
            var next = key + 1;
            if (next.Scale > 0 && GetOffsetFromMin(next) <= offset)
            {
                return next;
            }
            return key;
        }

        /// <summary>
        /// 获取每一个的值,
        /// </summary>
        /// <param name="key"></param>
        /// <returns>[left,top,width, height]</returns>
        public double[] Invalidate(PianoKey key)
        {
            Invalidate();
            var x = GetOffsetFromMin(key);
            var y = .0;
            var w = GetKeyWidth(key);
            var h = key.Scale > 0 ? BlackKeyHeight : WhiteKeyHeight;
            if (IsHorizontal)
            {
                return new double[] { x - Offset, y, w, h };
            }
            return new double[] {y, BarLength - Offset - x - w, h, w};
        }

        #region 计算方法


        /// <summary>
        /// 初始化数值，
        /// </summary>
        public void Invalidate()
        {
            if (IsInvalidated)
            {
                return;
            }
            MinOffset = GetOffsetFromZero(Min);
            MaxOffset = GetOffsetFromZero(Max);
            MaxToMin = MaxOffset - MinOffset;
            BarLength = MaxToMin + GetKeyWidth(Max);
            IsInvalidated = true;
        }

        public double GetKeyWidth(PianoKey key)
        {
            if (key.Scale > 0)
            {
                return BlackKeyWidth;
            }
            if (!IsSameGap)
            {
                return WhiteKeyWidth;
            }
            return (IsLargeWhiteKey(key) ? 2 : 1.5) * BlackKeyWidth;
        }

        /// <summary>
        /// 是否是居中的白键
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsLargeWhiteKey(PianoKey key)
        {
            return key.Scale == 0 && (key.Code == 2 || key.Code == 5 || key.Code == 6);
        }

        public double GetOffset(PianoKey key, PianoKey baseKey)
        {
            return GetOffsetFromZero(key) - GetOffsetFromZero(baseKey);
        }

        public double GetOffsetFromMin(PianoKey key)
        {
            return GetOffsetFromZero(key) - MinOffset;
        }

        public double GetOffsetToMax(PianoKey key)
        {
            return MaxOffset - GetOffsetFromZero(key);
        }

        public double GetOffsetFromZero(PianoKey key)
        {
            var bw = BlackKeyWidth;
            if (IsSameGap)
            {
                return key.ToKey127() * BlackKeyWidth - 
                    (key.Scale == 0 && key.Code != 1 && key.Code != 4 ? .5 * bw : 0);
            }
            var ww = WhiteKeyWidth;
            return ((key.Beat + 4) * 7 + key.Code - 1) * ww + (key.Scale > 0 ? (ww - bw / 2) : 0);
        }
        #endregion
    }

}
