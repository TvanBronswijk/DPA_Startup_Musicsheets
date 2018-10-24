using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Models.Wrappers
{
    class LilypondText
    {
        public string Text { get; }

        public LilypondText(string text)
        {
            this.Text = text;
        }
    }
}
