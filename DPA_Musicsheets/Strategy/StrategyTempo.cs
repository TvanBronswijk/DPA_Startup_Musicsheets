using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Models;
using Sanford.Multimedia.Midi;

namespace DPA_Musicsheets.Strategy
{
    class StrategyTempo : Strategy
    {
        public string execute(MidiEvent midiEvent, Midi midi)
        {
            IMidiMessage midiMessage = midiEvent.MidiMessage;
            var metaMessage = midiMessage as MetaMessage;
            StringBuilder lilypondContent = new StringBuilder();
            byte[] tempoBytes = metaMessage.GetBytes();
            int tempo = (tempoBytes[0] & 0xff) << 16 | (tempoBytes[1] & 0xff) << 8 | (tempoBytes[2] & 0xff);
            midi.bpm = 60000000 / tempo;
            lilypondContent.AppendLine($"\\tempo 4={ midi.bpm}");
            return lilypondContent.ToString();
        }
    }
}
