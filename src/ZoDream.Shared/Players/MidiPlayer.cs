using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using System;
using System.Collections.Generic;
using System.Text;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Players
{
    public class MidiPlayer : IPlayer
    {
        public MidiPlayer()
        {
            Ready();
        }

        public byte Volume { get; set; } = 127;
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

        public void Play(MidiChannel channel, PianoKey key)
        {
            Play((byte)channel, key);
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

        public void Stop(MidiChannel channel, PianoKey key)
        {
            Stop((byte)channel, key);
        }

        private void Ready()
        {
            Driver = OutputDevice.GetByIndex(0);
        }


        public void Dispose()
        {
            Driver?.Dispose();
        }
    }
}
