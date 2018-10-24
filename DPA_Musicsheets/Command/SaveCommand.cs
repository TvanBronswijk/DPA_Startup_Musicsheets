using DPA_Musicsheets.Converters.Strategy;
using DPA_Musicsheets.Managers;
using DPA_Musicsheets.ViewModels;
using Microsoft.Win32;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Command
{
    class SaveCommand : Command
    {
        MusicLoader _musicloader;

        public SaveCommand(MusicLoader musicloader)
        {
            _musicloader = musicloader;
        }

        public override void Execute()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = "PDF|*.pdf|MID|*.mid|LY|*.ly", Title = "Save" };
            if (saveFileDialog.ShowDialog() == true)
            {
                _musicloader.SaveFile(saveFileDialog.FileName);
            }
        }
    }
}
