using DPA_Musicsheets.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.State
{
    public abstract class MainViewModelState : ViewModelState<MainViewModel>
    {
        public MainViewModelState(MainViewModel context) : base(context)
        {
        }

        public abstract void Exit(CancelEventArgs args);
    }
}
