using DPA_Musicsheets.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DPA_Musicsheets.State
{
    public class RenderingState : MainViewModelState
    { 
        public RenderingState(MainViewModel context) : base(context)
        {
        }

        public override void Exit(CancelEventArgs args)
        {
            MessageBox.Show("Cannot exit while rendering!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            args.Cancel = true;
        }

        public override string ToString() => "Rendering...";
    }
}
