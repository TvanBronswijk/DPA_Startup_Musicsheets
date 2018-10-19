using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DPA_Musicsheets.Command
{
    class AddTime4Command : Command
    {
        TextBox _textBox;

        public AddTime4Command(TextBox textBox)
        {
            _textBox = textBox;
        }

        public override void Execute()
        {
            _textBox.Text = _textBox.Text.Insert(_textBox.SelectionStart, "\\time 4/4 ");
        }
    }
}
