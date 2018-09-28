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
            var context = new Context();
            StringBuilder lilypondContent = new StringBuilder();

            IMidiMessage midiMessage = midiEvent.MidiMessage;
            var metaMessage = midiMessage as MetaMessage;
            switch (metaMessage.MetaType)
            {
                case MetaType.TimeSignature:
                    context.setStrategy(new StrategyTimeSignature());
                    lilypondContent.Append(context.execute(midiEvent, midi));
                    break;
                case MetaType.Tempo:
                    context.setStrategy(new StrategyTempo());
                    lilypondContent.Append(context.execute(midiEvent, midi));
                    break;
                case MetaType.EndOfTrack:
                    context.setStrategy(new StrategyEndTrack());
                    lilypondContent.Append(context.execute(midiEvent, midi));
                    break;
                default: break;
            }
            return lilypondContent.ToString();
        }
    }
}
