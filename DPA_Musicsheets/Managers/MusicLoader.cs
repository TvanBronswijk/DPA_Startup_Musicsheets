using PSAMControlLibrary;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using DPA_Musicsheets.Converters.Strategy;

namespace DPA_Musicsheets.Managers
{
    public class MusicLoader
    {
        private LilypondConverter _lilypondConverter;
        private MidiConverter _midiConverter;
        private WPFConverter _wpfConverter;
        
        
        public void OpenFile(string fileName)
        {
            if (fileName.EndsWith(".mid"))
            {
                var file = _midiConverter.OpenFile(fileName);
                MidiLoaded.Invoke(this, file);


                var tokens = _midiConverter.Convert(file);
                LilypondLoaded.Invoke(this, _lilypondConverter.Convert(tokens));
                WPFLoaded.Invoke(this, _wpfConverter.Convert(tokens));
            }
            else
            {
                throw new NotImplementedException();
            }
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
