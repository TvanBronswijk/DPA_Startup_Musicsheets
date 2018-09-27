﻿using DPA_Musicsheets.Models;
using DPA_Musicsheets.Strategy;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DPA_Musicsheets.Converters.Strategy
{
    class MidiConverter : IMusicConverterStrategy<Sequence>
    {
        public Sequence MidiSequence { get; set; }

        private int _beatNote = 4;    // De waarde van een beatnote.
        private int _bpm = 120;       // Aantal beatnotes per minute.
        private int _beatsPerBar;


        public IEnumerable<MusicToken> Convert(Sequence src)
        {
            String lilyPondText = MidiToLilypond(src);
            lilyPondText = lilyPondText.Trim().ToLower().Replace("\r\n", " ").Replace("\n", " ").Replace("  ", " ");

            //converting it to lilypondToken
            LinkedList<MusicToken> tokens = new LinkedList<MusicToken>();

            foreach (string s in lilyPondText.Split(' ').Where(item => item.Length > 0))
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
            //converting to lilypondToken completed

        }

        public Sequence Convert(IEnumerable<MusicToken> tokens)
        {
            List<string> notesOrderWithCrosses = new List<string>() { "c", "cis", "d", "dis", "e", "f", "fis", "g", "gis", "a", "ais", "b" };
            int absoluteTicks = 0;

            Sequence sequence = new Sequence();

            Track metaTrack = new Track();
            sequence.Add(metaTrack);

            // Calculate tempo
            int speed = (60000000 / _bpm);
            byte[] tempo = new byte[3];
            tempo[0] = (byte)((speed >> 16) & 0xff);
            tempo[1] = (byte)((speed >> 8) & 0xff);
            tempo[2] = (byte)(speed & 0xff);
            metaTrack.Insert(0 /* Insert at 0 ticks*/, new MetaMessage(MetaType.Tempo, tempo));

            Track notesTrack = new Track();
            sequence.Add(notesTrack);

            foreach (MusicToken musicalSymbol in tokens)
            {
                switch (musicalSymbol.TokenKind)
                {
                    case MusicToken.Kind.Note:

                        // Calculate duration
                        double absoluteLength = 1.0 / (double)musicalSymbol.Length;
                        absoluteLength += (absoluteLength / 2.0) * musicalSymbol.Dots;

                        double relationToQuartNote = _beatNote / 4.0;
                        double percentageOfBeatNote = (1.0 / _beatNote) / absoluteLength;
                        double deltaTicks = (sequence.Division / relationToQuartNote) / percentageOfBeatNote;

                        // Calculate height
                        int noteHeight = notesOrderWithCrosses.IndexOf(musicalSymbol.Step.ToLower()) + ((musicalSymbol.Octave + 1) * 12);
                        noteHeight += musicalSymbol.Alter;
                        notesTrack.Insert(absoluteTicks, new ChannelMessage(ChannelCommand.NoteOn, 1, noteHeight, 90)); // Data2 = volume

                        absoluteTicks += (int)deltaTicks;
                        notesTrack.Insert(absoluteTicks, new ChannelMessage(ChannelCommand.NoteOn, 1, noteHeight, 0)); // Data2 = volume

                        break;
                    case MusicToken.Kind.Time:
                        byte[] timeSignature = new byte[4];
                        timeSignature[0] = (byte)_beatsPerBar;
                        timeSignature[1] = (byte)(Math.Log(_beatNote) / Math.Log(2));
                        metaTrack.Insert(absoluteTicks, new MetaMessage(MetaType.TimeSignature, timeSignature));
                        break;
                    default:
                        break;
                }
            }

            notesTrack.Insert(absoluteTicks, MetaMessage.EndOfTrackMessage);
            metaTrack.Insert(absoluteTicks, MetaMessage.EndOfTrackMessage);
            return sequence;
        }

        public Sequence OpenFile(string fileName)
        {
            MidiSequence = new Sequence();
            MidiSequence.Load(fileName);

            return MidiSequence;
        }

        public void SaveFile(string fileName)
        {
            MidiSequence.Save(fileName);
        }

        private String MidiToLilypond(Sequence src)
        {
            var context = new Context();
            //converting to lilypond string
            StringBuilder lilypondContent = new StringBuilder();
            lilypondContent.AppendLine("\\relative c' {");
            lilypondContent.AppendLine("\\clef treble");

            var midi = new Midi(src.Division, _beatsPerBar);

            for (int i = 0; i < src.Count; i++)
            {
                Track track = src[i];

                foreach (var midiEvent in track.Iterator())
                {
                    // TODO: Split this switch statements and create separate logic.
                    // We want to split this so that we can expand our functionality later with new keywords for example.
                    // Hint: Command pattern? Strategies? Factory method?

                    // stratagie pattern
                    // one for MESSAGETYPE
                    // one for METATYPE
                    // need to pass this class or the variables
                    // return value will be al string.

                    IMidiMessage midiMessage = midiEvent.MidiMessage;
                    switch (midiMessage.MessageType)
                    {
                        case MessageType.Meta:
                            context.setStrategy(new StrategyMeta());
                            lilypondContent.Append(context.execute(midiEvent, midi));
                            break;
                        case MessageType.Channel:
                            context.setStrategy(new StrategyChannel());
                            lilypondContent.Append(context.execute(midiEvent, midi));
                            break;
                    }
                }
            }

            lilypondContent.Append("}");

            return lilypondContent.ToString();
            //converting to lilypond string completed
        }
    }
}
