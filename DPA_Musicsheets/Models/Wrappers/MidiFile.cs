using Sanford.Multimedia.Midi;
using System;

namespace DPA_Musicsheets.Models.Wrappers
{
    public class MidiFile
    {
        public Sequence Sequence { get; }

        public MidiFile(Sequence sequence)
        {
            this.Sequence = sequence;
        }

    }
}
