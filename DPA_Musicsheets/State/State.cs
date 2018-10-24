using GalaSoft.MvvmLight;
using System.ComponentModel;

namespace DPA_Musicsheets.State
{
    public abstract class ViewModelState<T>
        where T : ViewModelBase
    {
        T context;
        public ViewModelState(T context)
        {
            this.context = context;
        }

        public abstract void exit(CancelEventArgs args);

        public override string ToString() => "";
    }
}
