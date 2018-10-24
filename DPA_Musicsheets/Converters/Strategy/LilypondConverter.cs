using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DPA_Musicsheets.Models;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

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
                    case "\r\n": break;
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

                if (s != "\r\n")
                {
                    if (tokens.Last != null)
                    {
                        tokens.Last.Value.NextToken = token;
                        token.PreviousToken = tokens.Last.Value;
                    }

                    tokens.AddLast(token);
                }
               
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
                    case MusicToken.Kind.Staff: lilypondText.Append( musicalSymbol.Value + " "); break;
                    case MusicToken.Kind.Clef: lilypondText.Append( musicalSymbol.Value + " "); break;
                    case MusicToken.Kind.Time: lilypondText.Append( musicalSymbol.Value + " "); break;
                    case MusicToken.Kind.Tempo: lilypondText.Append( musicalSymbol.Value + " "); break;
                    case MusicToken.Kind.Repeat: lilypondText.Append( musicalSymbol.Value + " "); break;
                    case MusicToken.Kind.Alternative: lilypondText.Append(musicalSymbol.Value + " "); break;
                    case MusicToken.Kind.SectionStart: lilypondText.Append("{ \r\n "); break;
                    case MusicToken.Kind.SectionEnd: lilypondText.Append(" }"); break;
                    case MusicToken.Kind.Bar: lilypondText.Append("|" + " \r\n "); break;
                    case MusicToken.Kind.Note: lilypondText.Append(musicalSymbol.Value + " "); break;
                    case MusicToken.Kind.Rest: lilypondText.Append(musicalSymbol.Value + " "); break;
                    case MusicToken.Kind.Unknown: lilypondText.Append(musicalSymbol.Value + " \r\n "); break;
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

        public void SaveFile(string fileName, IEnumerable<MusicToken> tokens)
        {
            if (fileName.EndsWith(".pdf"))
            {
                string lilypond = Convert<string>(tokens);
                int x = 50;
                int y = 100;

                PdfDocument document = new PdfDocument();
                document.Info.Title = "Lilypond";
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);


                XFont header = new XFont("Verdana", 20, XFontStyle.BoldItalic);
                gfx.DrawString("Lilypond", header, XBrushes.Black,
                new XRect(0, 50, page.Width, 0), XStringFormats.Center);

                foreach (string s in lilypond.Split(' ').Where(item => item.Length > 0))
                {
                    if (s == "\r\n")
                    {
                        x = 50;
                        y += 15;
                    }
                    else
                    {
                        XFont font = new XFont("Verdana", 10, XFontStyle.BoldItalic);
                        gfx.DrawString(s, font, XBrushes.Black,
                        new XRect(x, y, 0, 0), XStringFormats.TopLeft);
                        x += (s.Length * 8);
                    }
                }
                document.Save(fileName);
                Process.Start(fileName);// test
            }
            else
            {
                using (StreamWriter outputFile = new StreamWriter(fileName))
                {
                    outputFile.Write(Convert<String>(tokens));
                    outputFile.Close();
                }
            }
        }
    }
}
