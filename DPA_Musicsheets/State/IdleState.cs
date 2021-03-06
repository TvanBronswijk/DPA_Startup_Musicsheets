﻿using DPA_Musicsheets.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.State
{
    public class IdleState : MainViewModelState
    {
        public IdleState(MainViewModel context) : base(context)
        {
        }

        public override void Exit(CancelEventArgs args)
        {
            ViewModelLocator.Cleanup();
        }
    }
}
