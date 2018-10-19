using DPA_Musicsheets.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Command
{
    class OpenFileCommand : Command
    {
        MainViewModel _mvm;

        public OpenFileCommand(MainViewModel mvm)
        {
            _mvm = mvm;
        }
        public override void Execute()
        {
            _mvm.OpenFileCommand.Execute(new Object());
        }

    }
}
