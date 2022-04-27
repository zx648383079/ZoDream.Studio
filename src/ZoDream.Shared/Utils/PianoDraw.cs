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
            Offset = - GetOffsetFromMin(key);
        }

        /// <summary>
        /// 移动距离
        /// </summary>
        /// <param name="offset"></param>
        /// <returns>真实移动距离</returns>
        public double Move(double offset)
        {
            Invalidate();
            if (IsHorizontal)
            {
                offset *= -1;
            }
            var val = Math.Max(Math.Min(0, Offset + offset), - BarLength +
                (IsHorizontal ? BoxWidth : BoxHeight));
            var diff = val - val;
            Offset = val;
            return diff;
        }

        public PianoKey Get(double offset)
        {
            var w = WhiteKeyWidth;
            var min = Min;
            if (IsVertical)
            {
                offset = offset + MaxToMin - BoxHeight
                    + w;

            }
            var index = (offset -= Offset) / w;
            var key = new PianoKey
            {
                Beat = Convert.ToInt16(index / 7 - 4),
                Code = Convert.ToUInt16(index % 7)
            };
            var pre = key - 1;
            if (pre.Scale > 0 && GetOffsetFromMin(pre) + BlackKeyWidth > offset)
            {
                return pre;
            }
            var next = key + 1;
            if (next.Scale > 0 && GetOffsetFromMin(next) > offset)
            {
                return next;
            }
            return key;
        }

        /// <summary>
        /// 初始化数值，
        /// </summary>
        private void Invalidate()
        {
            if (IsInvalidated)
            {
                return;
            }
            MinOffset = GetOffsetFromZero(Min);
            MaxOffset = GetOffsetFromZero(Max);
            MaxToMin = MaxOffset - MinOffset;
            BarLength = MaxToMin + (Max.Scale > 0 ? BlackKeyWidth : WhiteKeyWidth);
            IsInvalidated = true;
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
            var w = key.Scale > 0 ? BlackKeyWidth : WhiteKeyWidth;
            var h = key.Scale > 0 ? BlackKeyHeight : WhiteKeyHeight;
            if (IsHorizontal)
            {
                return new double[] { x + Offset, y, w, h };
            }
            return new double[] {y, BoxHeight - x - w - Offset, h, w};
        }

        #region 计算方法
        private double GetOffset(PianoKey key, PianoKey baseKey)
        {
            return GetOffsetFromZero(key) - GetOffsetFromZero(baseKey);
        }

        private double GetOffsetFromMin(PianoKey key)
        {
            return GetOffsetFromZero(key) - MinOffset;
        }

        private double GetOffsetToMax(PianoKey key)
        {
            return MaxOffset - GetOffsetFromZero(key);
        }

        private double GetOffsetFromZero(PianoKey key)
        {
            var bw = BlackKeyWidth;
            var ww = WhiteKeyWidth;
            return ((key.Beat + 4) * 7 + key.Code - 1) * ww + (key.Scale > 0 ? (ww - bw / 2) : 0);
        }
        #endregion
    }

}
