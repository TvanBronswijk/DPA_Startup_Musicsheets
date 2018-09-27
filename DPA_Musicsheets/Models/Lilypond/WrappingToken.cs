using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Models.Lilypond
{
    class WrappingToken : Token
    {
        public List<Token> Children { get; set; }

        public override void Interpret()
        {
            throw new NotImplementedException();
        }
    }
}
