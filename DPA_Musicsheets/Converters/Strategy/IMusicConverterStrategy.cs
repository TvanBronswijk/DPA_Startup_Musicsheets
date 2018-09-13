using DPA_Musicsheets.Models;
using System.Collections.Generic;

namespace DPA_Musicsheets.Converters.Strategy
{
    interface IMusicConverterStrategy<T>
    {
        IEnumerable<MusicToken> Convert(T src);
        T Convert(IEnumerable<MusicToken> tokens);
    }
}
