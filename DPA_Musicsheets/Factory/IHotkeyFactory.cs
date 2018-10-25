using DPA_Musicsheets.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace DPA_Musicsheets.Factory
{
    interface IHotkeyFactory
    {

        IHotkeyCommand CreateHotkey(KeyboardDevice keyboard, TextBox textbox);
    }
}
