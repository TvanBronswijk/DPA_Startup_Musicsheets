using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Models
{
    public class Midi
    {

        public int division { get; set; }
        public int previousMidiKey { get; set; }
        public int previousNoteAbsoluteTicks { get; set; }
        public double percentageOfBarReached { get; set; }
        public bool startedNoteIsClosed { get; set; }

        public int beatNote { get; set; }// De waarde van een beatnote.
        public int bpm { get; set; }// Aantal beatnotes per minute.
        public int beatsPerBar { get; set; }

        public int absoluteTicks { get; set; }

        public Midi(int division, int beatsPerBar)
        {
            this.division = division;
            this.previousMidiKey = 60;
            this.previousNoteAbsoluteTicks = 0;
            this.percentageOfBarReached = 0;
            this.startedNoteIsClosed = true;
            this.beatNote = 4;
            this.bpm = 120;
            this.absoluteTicks = 0;
            this.beatsPerBar = beatsPerBar;
        }

        public string GetLilypondNoteLength(int nextNoteAbsoluteTicks, out double percentageOfBar)
        {
            int duration = 0;
            int dots = 0;

            double deltaTicks = nextNoteAbsoluteTicks - absoluteTicks;

            if (deltaTicks <= 0)
            {
                percentageOfBar = 0;
                return String.Empty;
            }

            double percentageOfBeatNote = deltaTicks / division;
            percentageOfBar = (1.0 / beatsPerBar) * percentageOfBeatNote;

            for (int noteLength = 32; noteLength >= 1; noteLength -= 1)
            {
                double absoluteNoteLength = (1.0 / noteLength);

                if (percentageOfBar <= absoluteNoteLength)
                {
                    if (noteLength < 2)
                        noteLength = 2;

                    int subtractDuration;

                    if (noteLength == 32)
                        subtractDuration = 32;
                    else if (noteLength >= 16)
                        subtractDuration = 16;
                    else if (noteLength >= 8)
                        subtractDuration = 8;
                    else if (noteLength >= 4)
                        subtractDuration = 4;
                    else
                        subtractDuration = 2;

                    if (noteLength >= 17)
                        duration = 32;
                    else if (noteLength >= 9)
                        duration = 16;
                    else if (noteLength >= 5)
                        duration = 8;
                    else if (noteLength >= 3)
                        duration = 4;
                    else
                        duration = 2;

                    double currentTime = 0;

                    while (currentTime < (noteLength - subtractDuration))
                    {
                        var addtime = 1 / ((subtractDuration / beatNote) * Math.Pow(2, dots));
                        if (addtime <= 0) break;
                        currentTime += addtime;
                        if (currentTime <= (noteLength - subtractDuration))
                        {
                            dots++;
                        }
                        if (dots >= 4) break;
                    }

                    break;
                }
            }

            return duration + new String('.', dots);
        }

        public string GetLilyNoteName(int midiKey)
        {
            int octave = (midiKey / 12) - 1;
            string name = "";
            switch (midiKey % 12)
            {
                case 0:
                    name = "c";
                    break;
                case 1:
                    name = "cis";
                    break;
                case 2:
                    name = "d";
                    break;
                case 3:
                    name = "dis";
                    break;
                case 4:
                    name = "e";
                    break;
                case 5:
                    name = "f";
                    break;
                case 6:
                    name = "fis";
                    break;
                case 7:
                    name = "g";
                    break;
                case 8:
                    name = "gis";
                    break;
                case 9:
                    name = "a";
                    break;
                case 10:
                    name = "ais";
                    break;
                case 11:
                    name = "b";
                    break;
            }

            int distance = midiKey - previousMidiKey;
            while (distance < -6)
            {
                name += ",";
                distance += 8;
            }

            while (distance > 6)
            {
                name += "'";
                distance -= 8;
            }

            return name;
        }

    }
}
