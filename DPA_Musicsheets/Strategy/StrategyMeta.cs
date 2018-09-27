using DPA_Musicsheets.Models;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Strategy
{
    class StrategyMeta : Strategy
    {
        public string execute(MidiEvent midiEvent, Midi midi)
        {
            StringBuilder lilypondContent = new StringBuilder();

            IMidiMessage midiMessage = midiEvent.MidiMessage;
            var metaMessage = midiMessage as MetaMessage;
            switch (metaMessage.MetaType)
            {
                case MetaType.TimeSignature:
                    byte[] timeSignatureBytes = metaMessage.GetBytes();
                    midi.beatNote = timeSignatureBytes[0];
                    midi.beatsPerBar = (int)(1 / Math.Pow(timeSignatureBytes[1], -2));
                    lilypondContent.AppendLine($"\\time { midi.beatNote}/{ midi.beatsPerBar}");
                    break;
                case MetaType.Tempo:
                    byte[] tempoBytes = metaMessage.GetBytes();
                    int tempo = (tempoBytes[0] & 0xff) << 16 | (tempoBytes[1] & 0xff) << 8 | (tempoBytes[2] & 0xff);
                    midi.bpm = 60000000 / tempo;
                    lilypondContent.AppendLine($"\\tempo 4={ midi.bpm}");
                    break;
                case MetaType.EndOfTrack:
                    if (midi.previousNoteAbsoluteTicks > 0)
                    {
                        // Finish the last notelength.
                        double percentageOfBar;
                        lilypondContent.Append(midi.GetLilypondNoteLength(midiEvent.AbsoluteTicks, out percentageOfBar));
                        lilypondContent.Append(" ");

                        midi.percentageOfBarReached += percentageOfBar;
                        if (midi.percentageOfBarReached >= 1)
                        {
                            lilypondContent.AppendLine("|");
                            percentageOfBar = percentageOfBar - 1;
                        }
                    }
                    break;
                default: break;
            }
            return lilypondContent.ToString();
        }
    }
}
