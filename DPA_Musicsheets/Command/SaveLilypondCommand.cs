using DPA_Musicsheets.Converters.Strategy;
using DPA_Musicsheets.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Command
{
    class SaveLilypondCommand : Command
    {
        LilypondViewModel _lvm;

        public SaveLilypondCommand(LilypondViewModel lvm)
        {
            _lvm = lvm;
        }

        public override void Execute()
        {
            _lvm.SaveAsCommand.Execute(new Object());
        }
    }
}
