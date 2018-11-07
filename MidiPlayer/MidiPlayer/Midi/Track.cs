using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiPlayer.Midi
{
    /// <summary>
    /// 轨道
    /// </summary>
    public class Track : IEnumerator, IEnumerable
    {
        const string CHUNK_ID = "MTrk";

        #region 属性

        /// <summary>
        /// 节奏 60 / 每拍
        /// </summary>
        public int Tempo { get; set; }
        /// <summary>
        /// 调号 1= C
        /// </summary>
        public string KeySignature { get; set; }
        /// <summary>
        /// 拍号中多少拍为一小节，分子
        /// </summary>
        public ushort TimeSignaturePerBar { get; set; }
        /// <summary>
        /// 拍号中多少音符为一拍，分母
        /// </summary>
        public ushort TimeSignaturePerTime { get; set; }
        /// <summary>
        /// 事件
        /// </summary>
        public IList<Key> EventList { get; set; } = new List<Key>();
        #endregion

        #region 支持循环
        private int position = -1;

        public object Current
        {
            get
            {

                try
                {
                    return EventList[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }

            }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)EventList).GetEnumerator();
        }

        public bool MoveNext()
        {
            position++;
            return (position < EventList.Count);
        }

        public void Reset()
        {
            position = 0;
        }
        #endregion


        /// <summary>
        /// 设置拍号
        /// </summary>
        /// <param name="time">多少拍为一小节</param>
        /// <param name="note">多少音符为一拍</param>
        public void SetTimeSignature(int time, int note)
        {
            TimeSignaturePerBar = Convert.ToUInt16(time);
            TimeSignaturePerTime = Convert.ToUInt16(note);
        }

        public void Append(Key item)
        {
            EventList.Add(item);
        }

        public void AppendKey(int code) => Append(new Key(code));

        public void AppendKey12(int code)
        {
            var item = new Key();
            item.SetKeyBy12(code);
            Append(item);
        }

        public void AppendKey127(int code)
        {
            var item = new Key();
            item.SetKeyBy127(code);
            Append(item);
        }

        public void AppendPrevious()
        {
            if (EventList.Count < 1) {
                return;
            }
            EventList[EventList.Count - 1].Speed ++;
        }
    }
}
