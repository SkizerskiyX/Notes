using System;
using System.Collections.Generic;
using System.Text;

namespace NoteModels.Models
{
    public class Note 
    {

      public const int MAX_TEXT_LENGHT = 250;
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public string Header { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsPinned { get; set; }
        public bool IsDeleted { get; set; }
        public Note(string header, string text)
        {
            Id = Guid.CreateVersion7();
            Header = header;
            Text = text;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            IsPinned = false;
            IsDeleted = false;
        }

        public Note(string header, string text, bool isPinned)
        {
            Id = Guid.CreateVersion7();
            Header = header;
            Text = text;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            IsPinned = isPinned;
            IsDeleted = false;
        }
        private Note() { }
    }
}
