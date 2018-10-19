using Microsoft.Win32;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Diagnostics;

namespace DPA_Musicsheets.Command
{
    class SavePDFCommand : Command
    {

        public override void Execute()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = "PDF|*.pdf" , Title = "Save as PDF"};
            if (saveFileDialog.ShowDialog() == true)
            {
                /*PdfDocument document = new PdfDocument();
                PdfPage page = document.AddPage();
                foreach(var staff in wpfstaffs)
                {
                    XGraphics gfx = XGraphics.FromPdfPage(page);
                    XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
                    gfx.DrawString("Hello, World!", font, XBrushes.Black,
                    new XRect(0, 0, page.Width, page.Height),XStringFormat.Center);
                }
                document.Save(saveFileDialog.FileName);
                Process.Start(saveFileDialog.FileName);// for testing*/
            }
        }
    }
}
