using NoteModels.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotesServices.Interfaces
{
    public interface INoteService 
    {
        Task<IEnumerable<Note>> GetAllAsync();
        Task<Note?> GetByIdAsync(Guid id);

        Task<Note> AddNoteAsync(Note note);

        Task<Note> UpdateNoteAsync(Guid id, Note note);

        Task DeleteNoteAsync(Guid id);
    }
}
