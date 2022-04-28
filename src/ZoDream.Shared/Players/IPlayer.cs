using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Players
{
    public interface IPlayer: IDisposable
    {
        public string[] ChannelItems { get; }

        /// <summary>
        /// 音量,0 -127
        /// </summary>
        public byte Volume { get; set; }

        public bool IsReady { get; }
        /// <summary>
        /// 准备
        /// </summary>
        /// <returns></returns>
        public Task ReadyAsync();

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
        /// 播放多久自动停止
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="key"></param>
        /// <param name="ms"></param>
        /// <returns></returns>
        public Task PlayAsync(byte channel, PianoKey key, uint ms);
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
    }
}
