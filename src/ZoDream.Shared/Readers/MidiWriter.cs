using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Readers
{
    public class MidiWriter : IFileWriter
    {
        public Task WriteAsync(string file, MusicItem data)
        {
            return Task.Factory.StartNew(() =>
            {
                To(data).Write(file);
            });
        }

        private MidiFile To(MusicItem data)
        {
            var midiFile = new MidiFile();
            var tempoMap = midiFile.GetTempoMap();

            var trackChunk = new TrackChunk();
            using (var notesManager = trackChunk.ManageNotes())
            {
                var length = LengthConverter.ConvertFrom(
                    2 * MusicalTimeSpan.Eighth.Triplet(),
                    0,
                    tempoMap);
                var note = new Melanchall.DryWetMidi.Interaction.Note(NoteName.A, 4, length);
                notesManager.Notes.Add(note);
            }

            midiFile.Chunks.Add(trackChunk);
            return midiFile;
        }
    }
}
