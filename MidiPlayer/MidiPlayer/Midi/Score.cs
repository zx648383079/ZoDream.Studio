using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiPlayer.Midi
{
    /// <summary>
    /// 简谱
    /// </summary>
    public class Score : IEnumerator, IEnumerable
    {
        #region 属性
        /// <summary>
        /// 曲名
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 作词
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// 作曲
        /// </summary>
        public string Composer { get; set; }
        /// <summary>
        /// 演唱
        /// </summary>
        public string Singer { get; set; }

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
        /// 内容
        /// </summary>
        public IList<Key> Tracks { get; set; } = new List<Key>();
        #endregion

        #region 支持循环
        private int position = -1;

        public object Current
        {
            get
            {

                try
                {
                    return Tracks[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }

            }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Tracks).GetEnumerator();
        }

        public bool MoveNext()
        {
            position++;
            return (position < Tracks.Count);
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

        public void AppendKey(int code)
        {

        }

        public void AppendKey12(int code)
        {

        }

        public void AppendKey127(int code)
        {

        }

        public void AppendPrevious()
        {

        }

    }
}
