using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using DPA_Musicsheets.Command;
using DPA_Musicsheets.Converters.Strategy;

namespace DPA_Musicsheets.Factory
{
    class HotkeyFactory : IHotkeyFactory
    {
        public IHotkeyCommand CreateHotkey(KeyboardDevice keyboard, TextBox textbox)
        {
            if (keyboard.IsKeyDown(Key.S) && keyboard.IsKeyDown(Key.LeftAlt) || keyboard.IsKeyDown(Key.S) && keyboard.IsKeyDown(Key.RightAlt))
            {
                new AddTempoCommand(textbox).Execute();
            }
            else if (keyboard.IsKeyDown(Key.C) && keyboard.IsKeyDown(Key.LeftAlt) || keyboard.IsKeyDown(Key.C) && keyboard.IsKeyDown(Key.RightAlt))
            {
                new AddClefTrebbleCommand(textbox).Execute();
            }
            else if (keyboard.IsKeyDown(Key.T) && keyboard.IsKeyDown(Key.D4) && keyboard.IsKeyDown(Key.LeftAlt) || keyboard.IsKeyDown(Key.T) && keyboard.IsKeyDown(Key.D4) && keyboard.IsKeyDown(Key.RightAlt))
            {
                new AddTime4Command(textbox).Execute();
            }
            else if (keyboard.IsKeyDown(Key.T) && keyboard.IsKeyDown(Key.D6) && keyboard.IsKeyDown(Key.LeftAlt) || keyboard.IsKeyDown(Key.T) && keyboard.IsKeyDown(Key.D6) && keyboard.IsKeyDown(Key.RightAlt))
            {
                new AddTime6Command(textbox).Execute();
            }
            else if (keyboard.IsKeyDown(Key.T) && keyboard.IsKeyDown(Key.D3) && keyboard.IsKeyDown(Key.LeftAlt) || keyboard.IsKeyDown(Key.T) && keyboard.IsKeyDown(Key.D3) && keyboard.IsKeyDown(Key.RightAlt))
            {
                new AddTime3Command(textbox).Execute();
            }
            else if (keyboard.IsKeyDown(Key.T) && keyboard.IsKeyDown(Key.LeftAlt) || keyboard.IsKeyDown(Key.T) && keyboard.IsKeyDown(Key.RightAlt))
            {
                new AddTimeCommand(textbox).Execute();
            }
            else if (keyboard.IsKeyDown(Key.B) && keyboard.IsKeyDown(Key.LeftAlt) || keyboard.IsKeyDown(Key.B) && keyboard.IsKeyDown(Key.RightAlt))
            {
                new AddBarLinesCommand(textbox).Execute();
            }
            return null;
        }
    }
}
