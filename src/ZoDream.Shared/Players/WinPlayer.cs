using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Players
{
    public class WinPlayer : IPlayer<PianoKey>
    {
        [DllImport("winmm.dll")]
        private static extern uint midiOutShortMsg(IntPtr hMidiOut, uint dwMsg);

        [DllImport("winmm.dll")]
        private static extern uint midiOutOpen(out IntPtr lphMidiOut, uint uDeviceID, uint dwCallBack, UInt32 dwInstance, UInt32 dwFlags);

        [DllImport("winmm.dll")]
        private static extern uint midiOutReset(IntPtr hMidiOut);

        [DllImport("winmm.dll")]
        private static extern uint midiOutClose(IntPtr hMidiOut);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hMidiOut"></param>
        /// <param name="status">状态</param>
        /// <param name="channel">频道0-127</param>
        /// <param name="scl">音阶</param>
        /// <param name="volume">音量</param>
        /// <returns></returns>
        public static uint MidiOutShortMsg(IntPtr hMidiOut, byte status, byte channel, byte scl, byte volume)
        {
            return midiOutShortMsg(hMidiOut, (uint)((status << 4) | channel | (scl << 8) | (volume << 16)));
        }


        private IntPtr Driver  = IntPtr.Zero;
        public string[] ChannelItems => Enum.GetNames(typeof(MidiChannel));
        public byte Volume { get; set; } = 127;

        public bool IsReady => Driver != IntPtr.Zero;


        public Task ReadyAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                uint r = midiOutOpen(out Driver, 0, 0, 0, 0);
                if (r != 0)
                {
                    Driver = IntPtr.Zero;
                }
            });
        }

        public void Play(byte channel, PianoKey key)
        {
            if (!IsReady)
            {
                return;
            }
            MidiOutShortMsg(Driver, 9, channel, key.ToKey127(), Volume);
        }

        public void Stop(byte channel, PianoKey key)
        {
            if (!IsReady)
            {
                return;
            }
            MidiOutShortMsg(Driver, 8, channel, key.ToKey127(), Volume);
        }

        public void Play(PianoKey key)
        {
            Play(MidiChannel.AcousticGrandPiano, key);
        }

        public async Task PlayAsync(byte channel, PianoKey key, uint ms)
        {
            Play(channel, key);
            await Task.Delay((int)ms);
            Stop(channel, key);
        }

        public void Stop(PianoKey key)
        {
            Stop(MidiChannel.AcousticGrandPiano, key);
        }

        public void Play(MidiChannel channel, PianoKey key)
        {
            Play((byte)channel, key);
        }

        public void Stop(MidiChannel channel, PianoKey key)
        {
            Stop((byte)channel, key);
        }

        public void Dispose()
        {
            if (IsReady)
            {
                midiOutClose(Driver);
            }
        }


    }
}
