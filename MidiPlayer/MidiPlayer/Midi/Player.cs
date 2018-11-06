using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Midi;

namespace MidiPlayer.Midi
{
    public class Player
    {
        public MidiSynthesizer Driver { get; set; }
        public byte Channel { get; set; }

        public byte Velocity { get; set; } = 127;

        private MidiNoteOnMessage current;

        public void Play(MidiNoteOnMessage message)
        {
            current = message;
            Send(message);
        }

        public async Task Play(Key item, Score score)
        {
            var note = item.ToTuple(score);
            await Play(note.Item1, note.Item2);
        }

        public async Task Play(byte note, int time)
        {
            var noteOn = new MidiNoteOnMessage(Channel, note, Velocity);
            Play(noteOn);
            await Task.Delay(time);
            Stop();
        }

        public void Play(byte note)
        {
            var noteOn = new MidiNoteOnMessage(Channel, note, Velocity);
            Play(noteOn);
        }

        public async Task Play(Score score)
        {
            foreach (Key item in score)
            {
                await Play(item, score);
            }
        }

        public void Stop()
        {
            if (current == null) {
                return;
            }
            var noteOff = new MidiNoteOffMessage(current.Channel, current.Note, current.Velocity);
            Send(noteOff);
            current = null;
        }

        public void Send(IMidiMessage message) {
            Driver.SendMessage(message);
        }

        public void Stop(MidiNoteOffMessage message)
        {
            Send(message);
        }

        public async void Ready() 
        {
            Driver = await MidiSynthesizer.CreateAsync();
        }

        public void Dispose()
        {
            Driver?.Dispose();
        }
    }
}
