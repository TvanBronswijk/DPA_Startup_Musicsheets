using DPA_Musicsheets.Models;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Strategy
{
    interface Strategy
    {
        String execute(MidiEvent midievent, Midi midi);
    }
}
