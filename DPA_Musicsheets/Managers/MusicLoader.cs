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

        public void LilyPondTextChanged(string text)
        {
            throw new NotImplementedException();
        }

        private void Convert()
        {
            LilypondLoaded.Invoke(this, null);
            WPFLoaded.Invoke(this, null);
            MidiLoaded.Invoke(this, null);
        }

        public event EventHandler<string> LilypondLoaded;
        public event EventHandler<List<MusicalSymbol>> WPFLoaded;
        public event EventHandler<Sequence> MidiLoaded;
    }
}
