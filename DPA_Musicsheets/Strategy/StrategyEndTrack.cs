using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Models;
using Sanford.Multimedia.Midi;

namespace DPA_Musicsheets.Strategy
{
    class StrategyEndTrack : Strategy
    {
        public string execute(MidiEvent midiEvent, Midi midi)
        {
            StringBuilder lilypondContent = new StringBuilder();
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
            return lilypondContent.ToString();
        }
    }
}
