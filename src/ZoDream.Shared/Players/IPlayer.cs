using System;
using System.Collections.Generic;
using System.Text;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Players
{
    public interface IPlayer: IDisposable
    {
        /// <summary>
        /// 音量,0 -127
        /// </summary>
        public byte Volume { get; set; }
        /// <summary>
        /// 按下键，播放按键对应声音
        /// </summary>
        /// <param name="key"></param>
        public void Play(PianoKey key);
        /// <summary>
        /// 按下键，播放按键对应声音
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="key"></param>
        public void Play(byte channel, PianoKey key);
        /// <summary>
        /// 按下键，播放按键对应声音
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="key"></param>
        public void Play(MidiChannel channel, PianoKey key);
        /// <summary>
        /// 释放键，停止播放按键对应声音
        /// </summary>
        /// <param name="key"></param>
        public void Stop(PianoKey key);
        /// <summary>
        /// 释放键，停止播放按键对应声音
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="key"></param>
        public void Stop(byte channel, PianoKey key);
        /// <summary>
        /// 释放键，停止播放按键对应声音
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="key"></param>
        public void Stop(MidiChannel channel, PianoKey key);
    }
}
