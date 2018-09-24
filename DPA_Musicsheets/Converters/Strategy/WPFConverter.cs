using DPA_Musicsheets.Models;
using PSAMControlLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Navigation;

namespace DPA_Musicsheets.Converters.Strategy
{
    class WPFConverter : IMusicConverterStrategy<List<MusicalSymbol>>
    {
        private static List<char> _notesorder = new List<char> {'c', 'd', 'e', 'f', 'g', 'a', 'b'};
        private readonly Dictionary<MusicToken.Kind, Func<MusicToken, MusicalSymbol>> _convertCommands;

        public WPFConverter()
        {
            _convertCommands = new Dictionary<MusicToken.Kind, Func<MusicToken, MusicalSymbol>>();

            _convertCommands.Add(MusicToken.Kind.Unknown, (musicToken) => null);
            _convertCommands.Add(MusicToken.Kind.Repeat,
                (musicToken) => new Barline {RepeatSign = RepeatSignType.Forward});
            _convertCommands.Add(MusicToken.Kind.SectionStart, (musicToken) =>
                musicToken.InAlternative && musicToken.PreviousToken.TokenKind != MusicToken.Kind.SectionEnd
                    ? new Barline {AlternateRepeatGroup = musicToken.AlternativeRepeatNumber}
                    : null);
            _convertCommands.Add(MusicToken.Kind.SectionEnd, (musicToken) =>
                (musicToken.InRepeat && musicToken.NextToken?.TokenKind != MusicToken.Kind.Alternative) ||
                (musicToken.InAlternative && musicToken.AlternativeRepeatNumber == 1)
                    ? new Barline
                    {
                        RepeatSign = RepeatSignType.Backward,
                        AlternateRepeatGroup = musicToken.AlternativeRepeatNumber
                    }
                    : null);
            _convertCommands.Add(MusicToken.Kind.Alternative, (musicToken) => null);
            _convertCommands.Add(MusicToken.Kind.Note, (musicToken) => null /*TODO*/);
            _convertCommands.Add(MusicToken.Kind.Rest, (musicToken) => new Rest((MusicalSymbolDuration)int.Parse(musicToken.Value[1].ToString())));
            _convertCommands.Add(MusicToken.Kind.Bar, (musicToken) => new Barline { AlternateRepeatGroup = musicToken.AlternativeRepeatNumber });
            _convertCommands.Add(MusicToken.Kind.Clef, (musicToken) =>
            {
                musicToken = musicToken.NextToken;
                switch (musicToken.Value)
                {
                    case "treble":
                        return new Clef(ClefType.GClef, 2);
                    case "bass":
                        return new Clef(ClefType.FClef, 4);
                    default:
                        throw new NotSupportedException($"Clef {musicToken.Value} is not supported.");
                }
            });
            _convertCommands.Add(MusicToken.Kind.Time, (musicToken) =>
            {
                musicToken = musicToken.NextToken;
                var times = musicToken.Value.Split('/');
                return new TimeSignature(TimeSignatureType.Numbers, uint.Parse(times[0]), uint.Parse(times[1]));
            });
            _convertCommands.Add(MusicToken.Kind.Tempo, (musicToken) => null /*Not Supported*/);
        }

        public IEnumerable<MusicToken> Convert(List<MusicalSymbol> src)
        {
            throw new NotImplementedException();
        }

        public List<MusicalSymbol> Convert(IEnumerable<MusicToken> tokens)
        {
            List<MusicalSymbol> symbols = new List<MusicalSymbol>();

            MusicToken currentToken = tokens.First();
            while (currentToken != null)
            {
                var newSymbol = _convertCommands[currentToken.TokenKind](currentToken);              
                if (newSymbol != null)
                    symbols.Add(newSymbol);
                
                if (currentToken.TokenKind == MusicToken.Kind.Alternative
                    || currentToken.TokenKind == MusicToken.Kind.Clef
                    || currentToken.TokenKind == MusicToken.Kind.Time)
                    currentToken = currentToken.NextToken;
                currentToken = currentToken.NextToken;
            }

            return symbols;
        }
    }
}