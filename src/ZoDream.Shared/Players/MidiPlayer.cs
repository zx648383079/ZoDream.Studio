using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Players
{
    public class MidiPlayer : IPlayer
    {
        public byte Volume { get; set; } = 127;
        public string[] ChannelItems => Enum.GetNames(typeof(MidiChannel));

        public bool IsReady { get; private set; } = false;
        private OutputDevice? Driver { get; set; }


        public void Play(PianoKey key)
        {
            Driver?.SendEvent(new NoteOnEvent(new SevenBitNumber(key.ToKey127()), new SevenBitNumber(Volume)));
        }

        public void Play(byte channel, PianoKey key)
        {
            Driver?.SendEvent(new NoteOnEvent(new SevenBitNumber(key.ToKey127()), new SevenBitNumber(Volume))
            {
                Channel = new FourBitNumber(channel),
            });
        }

        public async Task PlayAsync(byte channel, PianoKey key, uint ms)
        {
            Play(channel, key);
            await Task.Delay((int)ms);
            Stop(channel, key);
        }

        public void Stop(PianoKey key)
        {
            Driver?.SendEvent(new NoteOffEvent(new SevenBitNumber(key.ToKey127()), new SevenBitNumber(Volume)));
        }

        public void Stop(byte channel, PianoKey key)
        {
            Driver?.SendEvent(new NoteOffEvent(new SevenBitNumber(key.ToKey127()), new SevenBitNumber(Volume))
            {
                Channel = new FourBitNumber(channel),
            });
        }

        public Task ReadyAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                Driver = OutputDevice.GetByIndex(0);
                IsReady = true;
            });
        }


        public void Dispose()
        {
            Driver?.Dispose();
        }
    }
}
