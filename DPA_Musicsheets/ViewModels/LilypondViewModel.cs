using DPA_Musicsheets.Managers;
using DPA_Musicsheets.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DPA_Musicsheets.Models.Wrappers;
using DPA_Musicsheets.State;

namespace DPA_Musicsheets.ViewModels
{
    public class LilypondViewModel : ViewModelBase
    {
        private MusicLoader _musicLoader;
        private MainViewModel _mainViewModel { get; set; }

        private LilypondTextMemory _memory;

        /// <summary>
        /// This text will be in the textbox.
        /// It can be filled either by typing or loading a file so we only want to set previoustext when it's caused by typing.
        /// </summary>
        public string LilypondText
        {
            get
            {
                return _memory.Text;
            }
            set
            {
                if (!_waitingForRender && !_textChangedByLoad)
                {
                    _memory.Save();
                }
                _memory.Text = value;
                RaisePropertyChanged(() => LilypondText);
            }
        }

        private bool _textChangedByLoad = false;
        private DateTime _lastChange;
        private const int MILLISECONDS_BEFORE_CHANGE_HANDLED = 1500;
        private bool _waitingForRender = false;

        public LilypondViewModel(MainViewModel mainViewModel, MusicLoader musicLoader)
        {
            _mainViewModel = mainViewModel;
            _musicLoader = musicLoader;
            _memory = new LilypondTextMemory("Your lilypond text will appear here.");

            musicLoader.LilypondLoaded += (_, args) => LilypondTextLoaded(args);
        }

        public void LilypondTextLoaded(string text)
        {
            _textChangedByLoad = true;
            LilypondText = text;
            _textChangedByLoad = false;
        }

        /// <summary>
        /// This occurs when the text in the textbox has changed. This can either be by loading or typing.
        /// </summary>
        public ICommand TextChangedCommand => new RelayCommand<TextChangedEventArgs>((args) =>
        {
            // If we were typing, we need to do things.
            if (!_textChangedByLoad)
            {
                _waitingForRender = true;
                _lastChange = DateTime.Now;

                _mainViewModel.State = new RenderingState(_mainViewModel);

                Task.Delay(MILLISECONDS_BEFORE_CHANGE_HANDLED).ContinueWith((task) =>
                {
                    if ((DateTime.Now - _lastChange).TotalMilliseconds >= MILLISECONDS_BEFORE_CHANGE_HANDLED)
                    {
                        _waitingForRender = false;
                        UndoCommand.RaiseCanExecuteChanged();

                        _musicLoader.LilyPondTextChanged(LilypondText);
                        _mainViewModel.State = new UnsavedState(_mainViewModel);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext()); // Request from main thread.
            }
        });

        #region Commands for buttons like Undo, Redo and SaveAs
        public RelayCommand UndoCommand => new RelayCommand(() =>
        {
            _memory = _memory.Undo();
            RaisePropertyChanged(() => LilypondText);
        }, () => _memory.canUndo);

        public RelayCommand RedoCommand => new RelayCommand(() =>
        {
            _memory = _memory.Redo();
            RedoCommand.RaiseCanExecuteChanged();
            RaisePropertyChanged(() => LilypondText);
        }, () => _memory.canRedo);

        public ICommand SaveAsCommand => new RelayCommand(() =>
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = "Midi|*.mid|Lilypond|*.ly|PDF|*.pdf" };
            if (saveFileDialog.ShowDialog() == true)
            {
                string extension = Path.GetExtension(saveFileDialog.FileName);
                if(!_musicLoader.SaveFile(saveFileDialog.FileName))
                {
                    MessageBox.Show($"Extension {extension} is not supported.");
                }
                _mainViewModel.State = new IdleState(_mainViewModel);
            }
        });
        #endregion Commands for buttons like Undo, Redo and SaveAs
    }
}
