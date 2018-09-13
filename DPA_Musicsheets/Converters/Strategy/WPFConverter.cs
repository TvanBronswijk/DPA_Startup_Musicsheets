using DPA_Musicsheets.Models;
using PSAMControlLibrary;
using System;
using System.Collections.Generic;

namespace DPA_Musicsheets.Converters.Strategy
{
    class WPFConverter : IMusicConverterStrategy<List<MusicalSymbol>>
    {
        public IEnumerable<MusicToken> Convert(List<MusicalSymbol> src)
        {
            throw new NotImplementedException();
        }

        public List<MusicalSymbol> Convert(IEnumerable<MusicToken> tokens)
        {
            throw new NotImplementedException();
        }
    }
}
