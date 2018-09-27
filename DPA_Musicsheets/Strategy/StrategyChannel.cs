using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Models;
using Sanford.Multimedia.Midi;

namespace DPA_Musicsheets.Strategy
{
    public class StrategyChannel : Strategy
    {
        public string execute(MidiEvent midiEvent, Midi midi )
        {
            StringBuilder lilypondContent = new StringBuilder();

            var channelMessage = midiEvent.MidiMessage as ChannelMessage;
            if (channelMessage.Command == ChannelCommand.NoteOn)
            {
                if (channelMessage.Data2 > 0) // Data2 = loudness
                {
                    // Append the new note.
                    lilypondContent.Append(midi.GetLilyNoteName(channelMessage.Data1));

                    midi.previousMidiKey = channelMessage.Data1;
                    midi.startedNoteIsClosed = false;
                }
                else if (!midi.startedNoteIsClosed)
                {
                    // Finish the previous note with the length.
                    double percentageOfBar;
                    lilypondContent.Append(midi.GetLilypondNoteLength(midiEvent.AbsoluteTicks, out percentageOfBar));
                    midi.previousNoteAbsoluteTicks = midiEvent.AbsoluteTicks;
                    lilypondContent.Append(" ");

                    midi.percentageOfBarReached += percentageOfBar;
                    if (midi.percentageOfBarReached >= 1)
                    {
                        lilypondContent.AppendLine("|");
                        midi.percentageOfBarReached -= 1;
                    }
                    midi.startedNoteIsClosed = true;
                }
                else
                {
                    lilypondContent.Append("r");
                }
            }
            return lilypondContent.ToString();
        }
    }
}
