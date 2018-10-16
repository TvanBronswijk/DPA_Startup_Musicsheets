using PSAMControlLibrary;
using System;
using System.Collections.Generic;
using DPA_Musicsheets.Converters.Strategy;
using DPA_Musicsheets.Models.Wrappers;

namespace DPA_Musicsheets.Managers
{
    public class MusicLoader
    {
        private LilypondConverter _lilypondConverter = new LilypondConverter();
        private MidiConverter _midiConverter = new MidiConverter();
        private WPFConverter _wpfConverter = new WPFConverter();
        
        
        public void OpenFile(string fileName)
        {
            if (fileName.EndsWith(".mid"))
            {
                var file = _midiConverter.OpenFile(fileName);
                MidiLoaded.Invoke(this, new MidiFile(file));


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
            if (fileName.EndsWith(".mid"))
            {
                _midiConverter.SaveFile(fileName);
                return true;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void LilyPondTextChanged(string text)
        {
            var tokens = _lilypondConverter.Convert(text);
            WPFLoaded.Invoke(this, _wpfConverter.Convert(tokens));
            MidiLoaded.Invoke(this, new MidiFile(_midiConverter.Convert(tokens)));
        }

        private void Convert()
        {
            LilypondLoaded.Invoke(this, null);
            WPFLoaded.Invoke(this, null);
            MidiLoaded.Invoke(this, null);
        }

        public event EventHandler<string> LilypondLoaded;
        public event EventHandler<List<MusicalSymbol>> WPFLoaded;
        public event EventHandler<MidiFile> MidiLoaded;
    }
}
