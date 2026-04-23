using System;
using System.Collections.Generic;
using System.Text;

namespace NoteModels.Dto
{
    public class UpdateNoteDto
    {
        public string Header { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public bool isPinned { get; set; }
    }
}
