using DPA_Musicsheets.Models;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Strategy
{
    class Context
    {
        private Strategy _strategy;

        public Context(){ }

        public Context(Strategy strategy)
        {
            this._strategy = strategy;
        }

        public void setStrategy(Strategy strategy)
        {
            this._strategy = strategy;
        }

        public String execute(MidiEvent midievent, Midi midi)
        {
            return this._strategy.execute(midievent,midi);
        }

    }
}
