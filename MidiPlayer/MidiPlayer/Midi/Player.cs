using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Midi;
using System.Threading.Tasks;

namespace MidiPlayer.Midi
{
    public class Player
    {
        public MidiSynthesizer Driver { get; set; }
        public byte Channel { get; set; }

        public byte Velocity { get; set; }

        private MidiNoteOnMessage current;

        public void Play(MidiNoteOnMessage message)
        {
            current = message;
            Send(message);
        }

        public async void Play(Key item, Score score)
        {
            var note = item.ToTuple(score);
            var noteOn = new MidiNoteOnMessage(Channel, note.Item1, Velocity);
            Play(noteOn);
            await Task.Delay(note.Item2);
            Stop();
        }

        public async void Play(Score score)
        {
            foreach (Key item in score)
            {
                await Play(item, score);
            }
        }

        public void Stop()
        {
            if (!current) {
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
