using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DPA_Musicsheets.Models;

namespace DPA_Musicsheets.Converters.Strategy
{
    class LilypondConverter : IMusicConverterStrategy
    {
        public IEnumerable<MusicToken> Convert<T>(T src)
        {
            LinkedList<MusicToken> tokens = new LinkedList<MusicToken>();
            string str = (string)(object)src;

            foreach (string s in str.Split(' ').Where(item => item.Length > 0))
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

        public T Convert<T>(IEnumerable<MusicToken> tokens)
        {
            StringBuilder lilypondText = new StringBuilder();


            foreach (MusicToken musicalSymbol in tokens)
            {

                switch (musicalSymbol.TokenKind)
                {
                    case MusicToken.Kind.Staff: lilypondText.Append("\\" + musicalSymbol.Value + " "); break;
                    case MusicToken.Kind.Clef: lilypondText.Append("\\" + musicalSymbol.Value + " "); break;
                    case MusicToken.Kind.Time: lilypondText.Append("\\" + musicalSymbol.Value + " "); break;
                    case MusicToken.Kind.Tempo: lilypondText.Append("\\" + musicalSymbol.Value + " "); break;
                    case MusicToken.Kind.Repeat: lilypondText.Append("\\" + musicalSymbol.Value + " "); break;
                    case MusicToken.Kind.Alternative: lilypondText.Append(musicalSymbol.Value + " "); break;
                    case MusicToken.Kind.SectionStart: lilypondText.Append("{\r\n"); break;
                    case MusicToken.Kind.SectionEnd: lilypondText.Append(" }"); break;
                    case MusicToken.Kind.Bar: lilypondText.Append("|" + "\r\n"); break;
                    case MusicToken.Kind.Note: lilypondText.Append(musicalSymbol.Value + " "); break;
                    case MusicToken.Kind.Rest: lilypondText.Append(musicalSymbol.Value + " "); break;
                    case MusicToken.Kind.Unknown: lilypondText.Append(musicalSymbol.Value + "\r\n"); break;
                    default: break;
                }
            }

            return (T)(object)lilypondText.ToString();
        }

        public IEnumerable<MusicToken> OpenFile(string fileName)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var line in File.ReadAllLines(fileName))
            {
                sb.AppendLine(line);
            }

            String str = (String)(object)sb.ToString();
            return this.Convert(str);
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
