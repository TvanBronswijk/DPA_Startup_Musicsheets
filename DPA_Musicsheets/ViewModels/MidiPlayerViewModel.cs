using DPA_Musicsheets.Managers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace DPA_Musicsheets.ViewModels
{
    /// <summary>
    /// The viewmodel for playing midi sequences.
    /// It supports starting, stopping and restarting.
    /// </summary>
    public class MidiPlayerViewModel : ViewModelBase
    {
        private readonly MidiPlayer Player;

        public MidiPlayerViewModel(MusicLoader musicLoader)
        {
            Player = new MidiPlayer();
            musicLoader.MidiLoaded += (_, args) => Player.MidiFile = args;
        }

        private void UpdateButtons()
        {
            PlayCommand.RaiseCanExecuteChanged();
            PauseCommand.RaiseCanExecuteChanged();
            StopCommand.RaiseCanExecuteChanged();
        }

        

        #region buttons for play, stop, pause
        public RelayCommand PlayCommand => new RelayCommand(() =>
        {
            Player.Start();
            UpdateButtons();
        }, () => !Player.IsRunning);

        public RelayCommand StopCommand => new RelayCommand(() =>
        {
            Player.Stop();
            UpdateButtons();
        }, () => Player.IsRunning);

        public RelayCommand PauseCommand => new RelayCommand(() =>
        {
            Player.Pause();
            UpdateButtons();
        }, () => Player.IsRunning);

        #endregion buttons for play, stop, pause

        /// <summary>
        /// Stop the player and clear the sequence on cleanup.
        /// </summary>
        public override void Cleanup()
        {
            base.Cleanup();
            Player.Cleanup();
        }
    }
}
