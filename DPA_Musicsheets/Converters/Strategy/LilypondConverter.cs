using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DPA_Musicsheets.Models;
using Sanford.Multimedia.Midi;

namespace DPA_Musicsheets.Converters.Strategy
{
    class LilypondConverter : IMusicConverterStrategy<string>
    {
        public IEnumerable<MusicToken> Convert(string src)
        {
            LinkedList<MusicToken> tokens = new LinkedList<MusicToken>();

            foreach (string s in src.Split(' ').Where(item => item.Length > 0))
            {
                MusicToken token = new MusicToken()
                {
                    Value = s
                };

                switch (s)
                {
                    case "\\relative": token.TokenKind = MusicToken.Kind.Staff; break;
                    case "\\clef": token.TokenKind = MusicToken.Kind.Clef; break;
                    case "\\time": token.TokenKind = MusicToken.Kind.Time; break;
                    case "\\tempo": token.TokenKind = MusicToken.Kind.Tempo; break;
                    case "\\repeat": token.TokenKind = MusicToken.Kind.Repeat; break;
                    case "\\alternative": token.TokenKind = MusicToken.Kind.Alternative; break;
                    case "{": token.TokenKind = MusicToken.Kind.SectionStart; break;
                    case "}": token.TokenKind = MusicToken.Kind.SectionEnd; break;
                    case "|": token.TokenKind = MusicToken.Kind.Bar; break;
                    default: token.TokenKind = MusicToken.Kind.Unknown; break;
                }

                if (token.TokenKind == MusicToken.Kind.Unknown && new Regex(@"[~]?[a-g][,'eis]*[0-9]+[.]*").IsMatch(s))
                {
                    token.TokenKind = MusicToken.Kind.Note;
                }
                else if (token.TokenKind == MusicToken.Kind.Unknown && new Regex(@"r.*?[0-9][.]*").IsMatch(s))
                {
                    token.TokenKind = MusicToken.Kind.Rest;
                }

                if (tokens.Last != null)
                {
                    tokens.Last.Value.NextToken = token;
                    token.PreviousToken = tokens.Last.Value;
                }

                tokens.AddLast(token);
            }

            return tokens;

        }

        public string Convert(IEnumerable<MusicToken> tokens)
        {
            StringBuilder lilypondText = new StringBuilder();


            foreach (MusicToken musicalSymbol in tokens)
            {

                switch (musicalSymbol.TokenKind)
                {
                    case MusicToken.Kind.Staff: lilypondText.Append("\\relative " + musicalSymbol.Value); break;
                    case MusicToken.Kind.Clef: lilypondText.Append("\\clef " + musicalSymbol.Value); break;
                    case MusicToken.Kind.Time: lilypondText.Append("\\time " + musicalSymbol.Value); break;
                    case MusicToken.Kind.Tempo: lilypondText.Append("\\tempo " + musicalSymbol.Value); break;
                    case MusicToken.Kind.Repeat: lilypondText.Append("\\repeat " + musicalSymbol.Value); break;
                    case MusicToken.Kind.Alternative: lilypondText.Append("\\alternative " + musicalSymbol.Value); break;
                    case MusicToken.Kind.SectionStart: lilypondText.Append("{"); break;
                    case MusicToken.Kind.SectionEnd: lilypondText.Append("}"); break;
                    case MusicToken.Kind.Bar: lilypondText.Append("|"); break;
                    case MusicToken.Kind.Note: lilypondText.Append(musicalSymbol.Value); break;
                    case MusicToken.Kind.Rest: lilypondText.Append(musicalSymbol.Value); break;
                    default: break;
                }
            }

            return lilypondText.ToString();
        }

        public Sequence OpenFile(string fileName)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var line in File.ReadAllLines(fileName))
            {
                sb.AppendLine(line);
            }

            //this.LilypondText = sb.ToString();
            //this.LilypondViewModel.LilypondTextLoaded(this.LilypondText);
            return null;
        }

        public void SaveFile(string fileName)
        {
            using (StreamWriter outputFile = new StreamWriter(fileName))
            {
                //outputFile.Write(LilypondText);
                outputFile.Close();
            }
        }
    }
}
