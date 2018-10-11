using DPA_Musicsheets.Models;
using System.Collections.Generic;
using Sanford.Multimedia.Midi;

namespace DPA_Musicsheets.Converters.Strategy
{
    interface IMusicConverterStrategy
    {
        IEnumerable<MusicToken> Convert<T>(T src);
        T Convert<T>(IEnumerable<MusicToken> tokens);
        IEnumerable<MusicToken> OpenFile(string fileName);
        void SaveFile(string fileName);

    }
}
