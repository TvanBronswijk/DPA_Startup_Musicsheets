using PSAMControlLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Models.Wrappers
{
    public class WPFStaffs
    {
        public List<MusicalSymbol> Symbols { get; }

        public WPFStaffs(List<MusicalSymbol> symbols)
        {
            this.Symbols = symbols;
        }
    }
}
