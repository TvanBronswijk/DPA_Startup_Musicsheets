using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Models
{
    public class MusicToken
    {
        public static List<char> Notesorder = new List<char> {'c', 'd', 'e', 'f', 'g', 'a', 'b'};
        
        public Kind TokenKind { get; set; }
        public string Value { get; set; }

        public MusicToken NextToken { get; set; }
        public MusicToken PreviousToken { get; set; }

        public bool InRepeat => (TokenKind != Kind.SectionEnd && TokenKind != Kind.Alternative && TokenKind == Kind.Repeat) 
                                || (PreviousToken?.InRepeat ?? false);

        public bool InAlternative => (TokenKind != Kind.SectionEnd && TokenKind == Kind.Alternative)
                                || (PreviousToken?.InAlternative ?? false);
                            

        public int AlternativeRepeatNumber => (InAlternative && PreviousToken.AlternativeRepeatNumber == 1 ? 1 : 0) +
                                              (PreviousToken?.AlternativeRepeatNumber ?? 0);

        public int Octave
        {
            get
            {
                if(TokenKind != Kind.Note)
                {
                    return PreviousToken?.Octave ?? 4;
                }
                var previousNote = Previous(Kind.Note)?.Value[0] ?? 'c';
                var value = PreviousToken?.Octave ?? 4;
                var distanceWithPreviousNote =
                    Notesorder.IndexOf(Value[0]) - Notesorder.IndexOf(previousNote);
                if (distanceWithPreviousNote > 3) // Shorter path possible the other way around
                {
                    distanceWithPreviousNote -= 7; // The number of notes in an octave
                }
                else if (distanceWithPreviousNote < -3)
                {
                    distanceWithPreviousNote += 7; // The number of notes in an octave
                }

                if (distanceWithPreviousNote + Notesorder.IndexOf(previousNote) >= 7)
                {
                    value++;
                }
                else if (distanceWithPreviousNote + Notesorder.IndexOf(previousNote) < 0)
                {
                    value--;
                }

                // Force up or down.
                value += Value.Count(c => c == '\'');
                value -= Value.Count(c => c == ',');
                return value;
            }
        }

        public string Step => Value[0].ToString();
        public int Length => int.Parse(Regex.Match(Value, @"\d+").Value);
        public int Alter => Regex.Matches(Value, "is").Count - Regex.Matches(Value, "es|as").Count;
        public int Dots => Value.Count(c => c.Equals('.'));


        public MusicToken Previous(Kind tokenKind) =>
            PreviousToken?.TokenKind == tokenKind ? PreviousToken : PreviousToken?.Previous(tokenKind);

        /// <summary>
        /// This can be used to print our list and quickly see what it contains.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"Music Token: {TokenKind} - {Value}";

        /// <summary>
        /// These enums will be needed when loading an Lilypond file.
        /// These are the types we currently support. It is not an exhausted list.
        /// </summary>
        public enum Kind
        {
            Unknown,
            Note,
            Rest,
            Bar,
            Clef,
            Time,
            Tempo,
            Staff,
            Repeat,
            Alternative,
            SectionStart,
            SectionEnd
        }
    }
}
