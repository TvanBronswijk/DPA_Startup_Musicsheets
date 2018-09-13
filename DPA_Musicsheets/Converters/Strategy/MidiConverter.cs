using DPA_Musicsheets.Models;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;

namespace DPA_Musicsheets.Converters.Strategy
{
    class MidiConverter : IMusicConverterStrategy<Sequence>
    {
        public IEnumerable<MusicToken> Convert(Sequence src)
        {
            throw new NotImplementedException();
        }

        public Sequence Convert(IEnumerable<MusicToken> tokens)
        {
            throw new NotImplementedException();
        }
    }
}
