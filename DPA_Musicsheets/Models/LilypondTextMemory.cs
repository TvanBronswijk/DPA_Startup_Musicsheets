using DPA_Musicsheets.Models.Wrappers;
using System.Collections.Generic;

namespace DPA_Musicsheets.Models
{
    class LilypondTextMemory
    {
        private LilypondTextMemory previous;
        private LilypondText lilypondText;
        private LilypondTextMemory next;

        public string Text
        {
            get
            {
                return lilypondText.Text;
            }
            set
            {
                lilypondText = new LilypondText(value);
            }
        }

        public void Save()
        {
            var newSave = new LilypondTextMemory(lilypondText);
            newSave.previous = previous;
            if (previous != null)
            {
                previous.next = newSave;
            }
            newSave.next = this;
            previous = newSave;
            next = null;
        }

        public bool canUndo => previous != null;
        public bool canRedo => next != null;

        public LilypondTextMemory(string lilypondText) : this(new LilypondText(lilypondText))
        {
        }

        public LilypondTextMemory(LilypondText lilypondText)
        {
            this.lilypondText = lilypondText;
        }

        public LilypondTextMemory Undo() => previous ?? this;

        public LilypondTextMemory Redo() => next ?? this;
    }
}
