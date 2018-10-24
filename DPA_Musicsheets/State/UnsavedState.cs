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
    public class UnsavedState : ViewModelState<MainViewModel>
    {
        public UnsavedState(MainViewModel context) : base(context)
        {
        }

        public override void exit(CancelEventArgs args)
        {
            var result = MessageBox.Show("Are you sure you want to exit without saving?",
                "warning",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                ViewModelLocator.Cleanup();
            }
            else
            {
                args.Cancel = true;
            }
        }

        public override string ToString() => "Unsaved Changes";
    }
}
