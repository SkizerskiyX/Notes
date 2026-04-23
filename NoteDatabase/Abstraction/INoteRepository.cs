using NoteModels.Models;
using NoteDatabase.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteDatabase.Abstraction
{
    public interface INoteRepository
    {
        Task<IEnumerable<Note>> GetAllAsync();
        Task<Note?> GetByIdAsync(Guid id);

        Task<Note> AddNoteAsync(Note note);

        Task<Note> UpdateNoteAsync(Guid id, Note note);

        Task DeleteNoteAsync(Guid id);
    }
}
