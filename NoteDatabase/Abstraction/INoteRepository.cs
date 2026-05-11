using NoteModels.Models;
using NoteDatabase.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteDatabase.Abstraction
{
    public interface INoteRepository
    {
        Task<IEnumerable<Note>> GetAllAsync(Guid userId);
        Task<Note?> GetByIdAsync(Guid id, Guid userId);

        Task<Note> AddNoteAsync(Note note, Guid userId);

        Task<Note> UpdateNoteAsync(Guid id, Note note, Guid userId);

        Task DeleteNoteAsync(Guid id, Guid userId);
    }
}
