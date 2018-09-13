using System;
using System.Collections.Generic;
using DPA_Musicsheets.Models;

namespace DPA_Musicsheets.Converters.Strategy
{
    class LilypondConverter : IMusicConverterStrategy<string>
    {
        public IEnumerable<MusicToken> Convert(string src)
        {
            throw new NotImplementedException();
        }

        public string Convert(IEnumerable<MusicToken> tokens)
        {
            throw new NotImplementedException();
        }
    }
}
