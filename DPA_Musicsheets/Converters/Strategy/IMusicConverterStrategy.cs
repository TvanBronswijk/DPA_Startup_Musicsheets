using DPA_Musicsheets.Models;
using System.Collections.Generic;
using Sanford.Multimedia.Midi;

namespace DPA_Musicsheets.Converters.Strategy
{
    interface IMusicConverterStrategy<T>
    {
        IEnumerable<MusicToken> Convert(T src);
        T Convert(IEnumerable<MusicToken> tokens);
        T OpenFile(string fileName);
        void SaveFile(string fileName);

    }
}
