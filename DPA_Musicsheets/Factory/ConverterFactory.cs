using DPA_Musicsheets.Converters.Strategy;
using DPA_Musicsheets.Models;
using DPA_Musicsheets.Models.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Factory
{
    class ConverterFactory : IFactory
    {
        public IMusicConverterStrategy CreateConverter(string name)
        {
            if (name.EndsWith(".mid"))
            {
                return new MidiConverter();
            }
            if (name.EndsWith(".ly"))
            {
                return new LilypondConverter();
            }
            if (name.EndsWith(".pdf"))
            {
                return new LilypondConverter();
            }
            return null;
        }
    }
}
