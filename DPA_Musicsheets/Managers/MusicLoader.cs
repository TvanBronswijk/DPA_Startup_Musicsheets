using PSAMControlLibrary;
using System;
using System.Collections.Generic;
using DPA_Musicsheets.Converters.Strategy;
using DPA_Musicsheets.Models.Wrappers;
using DPA_Musicsheets.Factory;
using DPA_Musicsheets.Models;

namespace DPA_Musicsheets.Managers
{
    public class MusicLoader
    {
        private IEnumerable<MusicToken> tokens;
        private ConverterFactory converterFactory = new ConverterFactory();

        public void OpenFile(string fileName)
        {
            var converter = converterFactory.CreateConverter(fileName);
            tokens = converter.OpenFile(fileName);
            invoke();
        }

        internal bool SaveFile(string fileName)
        {
            var converter = converterFactory.CreateConverter(fileName);
            converter.SaveFile(fileName);
            return true;
        }

        public void LilyPondTextChanged(string text)
        {
            tokens = new LilypondConverter().Convert<string>(text);
            invoke();
        }
        public void invoke()
        {
            MidiLoaded.Invoke(this, new MidiConverter().Convert<MidiFile>(tokens));
            LilypondLoaded.Invoke(this, new LilypondConverter().Convert<String>(tokens));
            WPFLoaded.Invoke(this, new WPFConverter().Convert<WPFStaffs>(tokens));
        }
        private void Convert()
        {
            LilypondLoaded.Invoke(this, null);
            WPFLoaded.Invoke(this, null);
            MidiLoaded.Invoke(this, null);
        }

        public event EventHandler<string> LilypondLoaded;
        public event EventHandler<WPFStaffs> WPFLoaded;//needs to be changed to the wrapper
        public event EventHandler<MidiFile> MidiLoaded;
    }
}
