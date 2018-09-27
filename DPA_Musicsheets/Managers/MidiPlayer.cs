using DPA_Musicsheets.Models.Wrappers;
using Sanford.Multimedia.Midi;
using System;

namespace DPA_Musicsheets.Managers
{
    class MidiPlayer
    {
        private bool running;
        private readonly OutputDevice outputDevice;
        private readonly Sequencer sequencer;

        private MidiFile midiFile;

        public MidiFile MidiFile
        {
            get
            {
                return MidiFile;
            }
            set
            {
                midiFile = value;
                sequencer.Sequence = value.Sequence;
            }
        }

        public MidiPlayer() : this(new OutputDevice(0), new Sequencer())
        {

        }

        public MidiPlayer(OutputDevice device, Sequencer sequencer)
        {
            this.outputDevice = device;
            this.sequencer = sequencer;
            this.sequencer.ChannelMessagePlayed += ChannelMessagePlayed;
            // Wanneer de sequence klaar is moeten we alles closen en stoppen.
            this.sequencer.PlayingCompleted += (playingSender, playingEvent) =>
            {
                sequencer.Stop();
                running = false;
            };
        }

        // Wanneer een channelmessage langskomt sturen we deze direct door naar onze audio.
        // Channelmessages zijn tonen met commands als NoteOn en NoteOff
        // In midi wordt elke noot gespeeld totdat NoteOff is benoemd. Wanneer dus nooit een NoteOff komt nadat die een NoteOn heeft gehad
        // zal deze note dus oneindig lang blijven spelen.
        private void ChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
        {
            try
            {
                outputDevice.Send(e.Message);
            }
            catch (Exception ex) when (ex is ObjectDisposedException || ex is OutputDeviceException)
            {
                // Don't crash when we can't play
                // We have to do it this way because IsDisposed on
                // _outDevice may be false when it is being disposed
                // so this is the only safe way to prevent race conditions
            }
        }

        public bool IsRunning => running;

        public void Start()
        {
            if (!running)
            {
                running = true;
                sequencer.Continue();
            }
        }

        public void Stop()
        {
            running = false;
            sequencer.Stop();
            sequencer.Position = 0;
        }

        public void Pause()
        {
            running = false;
            sequencer.Stop();
        }

        public void Cleanup()
        {
            sequencer.Stop();
            sequencer.Dispose();
            outputDevice.Dispose();
        }
    }
}
