using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DPA_Musicsheets.Command
{
    class AddBarLinesCommand : Command
    {
        TextBox _textBox;

        public AddBarLinesCommand(TextBox textBox)
        {
            _textBox = textBox;
        }

        public override void Execute() // werkt nog niet 100 procent
        {
            String previous = "";
            StringBuilder lilypondText = new StringBuilder();
            String lilypond = _textBox.Text;
            foreach (string s in lilypond.Split(' ').Where(item => item.Length > 0))
            {
                if (s.Contains("\r\n"))
                {
                    Match match = Regex.Match(previous, @"[a-z][0-9]");
                    if (match.Success) lilypondText.Append("|");
                    if(s == "\r\n")
                    {
                        lilypondText.Append(s + " ");
                    }
                    else
                    {
                        var tmp = s.Length;
                        lilypondText.Append(" " + s.Substring(2) + " ");
                    }
                }else lilypondText.Append(s + " ");
                previous = s;
                
            }
            _textBox.Text = lilypondText.ToString();
        }
    }
}
