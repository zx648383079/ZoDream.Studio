using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midi
{
    /// <summary>
    /// 单个音节的数据
    /// </summary>
    public class MidiTime
    {
        private int _midi;
        /// <summary>
        /// 音阶
        /// </summary>
        public int Midi
        {
            get { return _midi; }
            set { _midi = value; }
        }

        private double _time;
        /// <summary>
        /// 时间
        /// </summary>
        public double Time
        {
            get { return _time; }
            set { _time = value; }
        }
        /// <summary>
        /// 无数据构造函数
        /// </summary>
        public MidiTime()
        {

        }
        /// <summary>
        /// 有数据构造函数
        /// </summary>
        /// <param name="midi">音阶</param>
        /// <param name="time">时间</param>
        public MidiTime(int midi,double time)
        {
            this._time = time;
            this._midi = midi;
        }
    }
}
