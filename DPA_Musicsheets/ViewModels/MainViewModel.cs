using DPA_Musicsheets.Command;
using DPA_Musicsheets.Managers;
using DPA_Musicsheets.State;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace DPA_Musicsheets.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private MainViewModelState state;
        private string _fileName;
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
                RaisePropertyChanged(() => FileName);
            }
        }

        public MainViewModelState State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
                RaisePropertyChanged(() => State);
            }
        }

        private MusicLoader _musicLoader;

        public MainViewModel(MusicLoader musicLoader)
        {
            state = new IdleState(this);
            _musicLoader = musicLoader;
            FileName = @"Files/Alle-eendjes-zwemmen-in-het-water.mid";
        }

        public ICommand OpenFileCommand => new RelayCommand(() =>
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "Midi or LilyPond files (*.mid *.ly)|*.mid;*.ly" };
            if (openFileDialog.ShowDialog() == true)
            {
                FileName = openFileDialog.FileName;
            }
        });

        public ICommand LoadCommand => new RelayCommand(() =>
        {
            _musicLoader.OpenFile(FileName);
        });

        #region Focus and key commands, these can be used for implementing hotkeys
        public ICommand OnLostFocusCommand => new RelayCommand(() =>
        {
            Console.WriteLine("Maingrid Lost focus");
        });

        public ICommand OnKeyDownCommand => new RelayCommand<KeyEventArgs>((e) =>
        {
            Console.WriteLine($"Key down: {e.Key}");
            var keyboard = e.KeyboardDevice;
            if(keyboard.IsKeyDown(Key.O)&& keyboard.IsKeyDown(Key.LeftCtrl) || keyboard.IsKeyDown(Key.O) && keyboard.IsKeyDown(Key.RightCtrl))
            {
                new OpenFileCommand(this).Execute();
            }
            else if (keyboard.IsKeyDown(Key.S) && keyboard.IsKeyDown(Key.LeftCtrl) || keyboard.IsKeyDown(Key.S) && keyboard.IsKeyDown(Key.RightCtrl))
            {
                new SaveCommand(_musicLoader).Execute();
            }
        });

        public ICommand OnKeyUpCommand => new RelayCommand(() =>
        {
            Console.WriteLine("Key Up");
        });

        public ICommand OnWindowClosingCommand => new RelayCommand<CancelEventArgs>((e) =>
        {
            state.Exit(e);
        });
        #endregion Focus and key commands, these can be used for implementing hotkeys
    }
}
