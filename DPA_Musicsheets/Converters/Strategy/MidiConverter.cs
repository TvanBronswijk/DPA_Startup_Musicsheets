using DPA_Musicsheets.Models;
using PSAMControlLibrary;
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
                        var note = musicalSymbol;

                        // Calculate duration
                        double absoluteLength = 1.0 / (double)note.Length;
                        absoluteLength += (absoluteLength / 2.0) * note.Dots;

                        double relationToQuartNote = _beatNote / 4.0;
                        double percentageOfBeatNote = (1.0 / _beatNote) / absoluteLength;
                        double deltaTicks = (sequence.Division / relationToQuartNote) / percentageOfBeatNote;

                        // Calculate height
                        int noteHeight = notesOrderWithCrosses.IndexOf(note.Step.ToLower()) + ((note.Octave + 1) * 12);
                        noteHeight += note.Alter;
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
            //needs to be done
            //this.LilypondText = LoadMidiIntoLilypond(MidiSequence);
            //this.LilypondViewModel.LilypondTextLoaded(this.LilypondText);
        }

        public void SaveFile(string fileName)
        {
            MidiSequence.Save(fileName);
        }

        private String MidiToLilypond(Sequence src)
        {
            //converting to lilypond string
            StringBuilder lilypondContent = new StringBuilder();
            lilypondContent.AppendLine("\\relative c' {");
            lilypondContent.AppendLine("\\clef treble");

            int division = src.Division;
            int previousMidiKey = 60; // Central C;
            int previousNoteAbsoluteTicks = 0;
            double percentageOfBarReached = 0;
            bool startedNoteIsClosed = true;

            for (int i = 0; i < src.Count; i++)
            {
                Track track = src[i];

                foreach (var midiEvent in track.Iterator())
                {
                    IMidiMessage midiMessage = midiEvent.MidiMessage;
                    // TODO: Split this switch statements and create separate logic.
                    // We want to split this so that we can expand our functionality later with new keywords for example.
                    // Hint: Command pattern? Strategies? Factory method?
                    switch (midiMessage.MessageType)
                    {
                        case MessageType.Meta:
                            var metaMessage = midiMessage as MetaMessage;
                            switch (metaMessage.MetaType)
                            {
                                case MetaType.TimeSignature:
                                    byte[] timeSignatureBytes = metaMessage.GetBytes();
                                    _beatNote = timeSignatureBytes[0];
                                    _beatsPerBar = (int)(1 / Math.Pow(timeSignatureBytes[1], -2));
                                    lilypondContent.AppendLine($"\\time {_beatNote}/{_beatsPerBar}");
                                    break;
                                case MetaType.Tempo:
                                    byte[] tempoBytes = metaMessage.GetBytes();
                                    int tempo = (tempoBytes[0] & 0xff) << 16 | (tempoBytes[1] & 0xff) << 8 | (tempoBytes[2] & 0xff);
                                    _bpm = 60000000 / tempo;
                                    lilypondContent.AppendLine($"\\tempo 4={_bpm}");
                                    break;
                                case MetaType.EndOfTrack:
                                    if (previousNoteAbsoluteTicks > 0)
                                    {
                                        // Finish the last notelength.
                                        double percentageOfBar;
                                        lilypondContent.Append(GetLilypondNoteLength(previousNoteAbsoluteTicks, midiEvent.AbsoluteTicks, division, _beatNote, _beatsPerBar, out percentageOfBar));
                                        lilypondContent.Append(" ");

                                        percentageOfBarReached += percentageOfBar;
                                        if (percentageOfBarReached >= 1)
                                        {
                                            lilypondContent.AppendLine("|");
                                            percentageOfBar = percentageOfBar - 1;
                                        }
                                    }
                                    break;
                                default: break;
                            }
                            break;
                        case MessageType.Channel:
                            var channelMessage = midiEvent.MidiMessage as ChannelMessage;
                            if (channelMessage.Command == ChannelCommand.NoteOn)
                            {
                                if (channelMessage.Data2 > 0) // Data2 = loudness
                                {
                                    // Append the new note.
                                    lilypondContent.Append(GetLilyNoteName(previousMidiKey, channelMessage.Data1));

                                    previousMidiKey = channelMessage.Data1;
                                    startedNoteIsClosed = false;
                                }
                                else if (!startedNoteIsClosed)
                                {
                                    // Finish the previous note with the length.
                                    double percentageOfBar;
                                    lilypondContent.Append(GetLilypondNoteLength(previousNoteAbsoluteTicks, midiEvent.AbsoluteTicks, division, _beatNote, _beatsPerBar, out percentageOfBar));
                                    previousNoteAbsoluteTicks = midiEvent.AbsoluteTicks;
                                    lilypondContent.Append(" ");

                                    percentageOfBarReached += percentageOfBar;
                                    if (percentageOfBarReached >= 1)
                                    {
                                        lilypondContent.AppendLine("|");
                                        percentageOfBarReached -= 1;
                                    }
                                    startedNoteIsClosed = true;
                                }
                                else
                                {
                                    lilypondContent.Append("r");
                                }
                            }
                            break;
                    }
                }
            }

            lilypondContent.Append("}");

            return lilypondContent.ToString();
            //converting to lilypond string completed
        }

        private string GetLilypondNoteLength(int absoluteTicks, int nextNoteAbsoluteTicks, int division, int beatNote, int beatsPerBar, out double percentageOfBar)
        {
            int duration = 0;
            int dots = 0;

            double deltaTicks = nextNoteAbsoluteTicks - absoluteTicks;

            if (deltaTicks <= 0)
            {
                percentageOfBar = 0;
                return String.Empty;
            }

            double percentageOfBeatNote = deltaTicks / division;
            percentageOfBar = (1.0 / beatsPerBar) * percentageOfBeatNote;

            for (int noteLength = 32; noteLength >= 1; noteLength -= 1)
            {
                double absoluteNoteLength = (1.0 / noteLength);

                if (percentageOfBar <= absoluteNoteLength)
                {
                    if (noteLength < 2)
                        noteLength = 2;

                    int subtractDuration;

                    if (noteLength == 32)
                        subtractDuration = 32;
                    else if (noteLength >= 16)
                        subtractDuration = 16;
                    else if (noteLength >= 8)
                        subtractDuration = 8;
                    else if (noteLength >= 4)
                        subtractDuration = 4;
                    else
                        subtractDuration = 2;

                    if (noteLength >= 17)
                        duration = 32;
                    else if (noteLength >= 9)
                        duration = 16;
                    else if (noteLength >= 5)
                        duration = 8;
                    else if (noteLength >= 3)
                        duration = 4;
                    else
                        duration = 2;

                    double currentTime = 0;

                    while (currentTime < (noteLength - subtractDuration))
                    {
                        var addtime = 1 / ((subtractDuration / beatNote) * Math.Pow(2, dots));
                        if (addtime <= 0) break;
                        currentTime += addtime;
                        if (currentTime <= (noteLength - subtractDuration))
                        {
                            dots++;
                        }
                        if (dots >= 4) break;
                    }

                    break;
                }
            }

            return duration + new String('.', dots);
        }

        public string GetLilyNoteName(int previousMidiKey, int midiKey)
        {
            int octave = (midiKey / 12) - 1;
            string name = "";
            switch (midiKey % 12)
            {
                case 0:
                    name = "c";
                    break;
                case 1:
                    name = "cis";
                    break;
                case 2:
                    name = "d";
                    break;
                case 3:
                    name = "dis";
                    break;
                case 4:
                    name = "e";
                    break;
                case 5:
                    name = "f";
                    break;
                case 6:
                    name = "fis";
                    break;
                case 7:
                    name = "g";
                    break;
                case 8:
                    name = "gis";
                    break;
                case 9:
                    name = "a";
                    break;
                case 10:
                    name = "ais";
                    break;
                case 11:
                    name = "b";
                    break;
            }

            int distance = midiKey - previousMidiKey;
            while (distance < -6)
            {
                name += ",";
                distance += 8;
            }

            while (distance > 6)
            {
                name += "'";
                distance -= 8;
            }

            return name;
        }
    }
}
