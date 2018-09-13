﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Models
{
    public class MusicToken
    {
        public Kind TokenKind { get; set; }
        public string Value { get; set; }

        public MusicToken NextToken { get; set; }

        public MusicToken PreviousToken { get; set; }

        /// <summary>
        /// This can be used to print our list and quickly see what it contains.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"Music Token: {TokenKind} - {Value}";

        /// <summary>
        /// These enums will be needed when loading an Lilypond file.
        /// These are the types we currently support. It is not an exhausted list.
        /// </summary>
        public enum Kind
        {
            Unknown,
            Note,
            Rest,
            Bar,
            Clef,
            Time,
            Tempo,
            Staff,
            Repeat,
            Alternative,
            SectionStart,
            SectionEnd
        }
    }
}