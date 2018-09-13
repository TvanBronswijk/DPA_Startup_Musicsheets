using PSAMControlLibrary;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;

namespace DPA_Musicsheets.Managers
{
    public class MusicLoader
    {
        public void OpenFile(string fileName)
        {
            throw new NotImplementedException();
        }

        internal bool SaveFile(string fileName)
        {
            throw new NotImplementedException();
        }

        public void LilyPondTextChanged(string text) => LilypondLoaded.Invoke(this, text);

        public event EventHandler<string> LilypondLoaded;
        public event EventHandler<List<MusicalSymbol>> WPFLoaded;
        public event EventHandler<Sequence> MidiLoaded;
    }
}
