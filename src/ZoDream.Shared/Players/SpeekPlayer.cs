using System;
using System.Collections.Generic;
using System.Speech.Synthesis;
using System.Text;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Players
{
    public class SpeekPlayer : IPlayer
    {

        private SpeechSynthesizer? Driver;

        public byte Volume { get; set; }

        public void Play(PianoKey key)
        {
            //Driver.Speak()
        }

        public void Play(byte channel, PianoKey key)
        {
            throw new NotImplementedException();
        }

        public void Play(MidiChannel channel, PianoKey key)
        {
            throw new NotImplementedException();
        }

        public void Stop(PianoKey key)
        {
            throw new NotImplementedException();
        }

        public void Stop(byte channel, PianoKey key)
        {
            throw new NotImplementedException();
        }

        public void Stop(MidiChannel channel, PianoKey key)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Driver?.Dispose();
        }


        private void Ready()
        {
            Driver = new SpeechSynthesizer();
        }
    }
}
