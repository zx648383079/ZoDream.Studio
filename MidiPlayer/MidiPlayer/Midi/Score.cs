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

        const string CHUNK_ID = "MThd";

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
        /// 每一拍经过的Tick数，范围从48到960
        /// </summary>
        public ushort TicksPerBeat { get; set; } = 960;
        /// <summary>
        /// 文件的格式 0－单轨 1－多规，同步 2－多规，异步
        /// </summary>
        public ushort FormatType { get; set; } = 0;
        /// <summary>
        /// 轨道列表
        /// </summary>
        public IList<Track> TrackList { get; set; } = new List<Track>();
        #endregion

        #region 支持循环
        private int position = -1;

        public object Current
        {
            get
            {

                try
                {
                    return TrackList[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }

            }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)TrackList).GetEnumerator();
        }

        public bool MoveNext()
        {
            position++;
            return (position < TrackList.Count);
        }

        public void Reset()
        {
            position = 0;
        }
        #endregion

        public void Append(Track item)
        {
            TrackList.Add(item);
        }
    }
}
