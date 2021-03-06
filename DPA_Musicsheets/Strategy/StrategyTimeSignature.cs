﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPA_Musicsheets.Models;
using Sanford.Multimedia.Midi;

namespace DPA_Musicsheets.Strategy
{
    class StrategyTimeSignature : Strategy
    {
        public string execute(MidiEvent midiEvent, Midi midi)
        {
            IMidiMessage midiMessage = midiEvent.MidiMessage;
            var metaMessage = midiMessage as MetaMessage;

            StringBuilder lilypondContent = new StringBuilder();
            byte[] timeSignatureBytes = metaMessage.GetBytes();
            midi.beatNote = timeSignatureBytes[0];
            midi.beatsPerBar = (int)(1 / Math.Pow(timeSignatureBytes[1], -2));
            lilypondContent.AppendLine($"\\time { midi.beatNote}/{ midi.beatsPerBar}");
            return lilypondContent.ToString();
        }
    }
}
