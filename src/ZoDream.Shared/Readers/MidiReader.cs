using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;
using ZoDream.Shared.Utils;

namespace ZoDream.Shared.Readers
{
    public class MidiReader : IFileReader
    {
        public Task<MusicItem?> ReadAsync(string file)
        {
            return Task.Factory.StartNew(() =>
            {
                return From(MidiFile.Read(file));
            });
        }

        private MusicItem? From(MidiFile? midi)
        {
            if (midi == null)
            {
                return null;
            }
            var data = new MusicItem();
            foreach (var trackChunk in midi.GetTrackChunks())
            {
                using var notesManager = trackChunk.ManageNotes();
            }
            return data;
        }



    }
}
