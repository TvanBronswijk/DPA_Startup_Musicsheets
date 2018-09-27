using System.Collections.Generic;

namespace DPA_Musicsheets.Models.Lilypond
{
    class LilypondContext
    {
        public Dictionary<string, int> Variables { get; set; }

        public LilypondContext()
        {
            Variables = new Dictionary<string, int>();
        }
    }
}
