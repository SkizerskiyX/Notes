using Microsoft.EntityFrameworkCore;
using NoteModels.Models;
using NoteDatabase.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using NoteDatabase.DbContext;

namespace NoteDatabase.Repositories
{
    public class Repository : INoteRepository
    {
        private readonly NoteDbContext _context;
        public Repository(NoteDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Note>> GetAllAsync()
        {
            var getAll = await _context.Notes
                .AsNoTracking()
                .ToListAsync();
            return getAll;
        }
        public async Task<Note?> GetByIdAsync(Guid id)
        {
            return await _context.Notes.AsNoTracking().SingleOrDefaultAsync(n => n.Id == id);
              
        }
        public async Task<Note> AddNoteAsync(Note note)
        {
             _context.Notes.Add(note);
            await _context.SaveChangesAsync();
            return note;
        }
        public async Task<Note> UpdateNoteAsync(Guid id, Note note)
        {
            var entity = await _context.Notes.SingleOrDefaultAsync(n => n.Id == id);

            if (entity == null)
                throw new InvalidOperationException($"Note with id '{id}' not found.");

            entity.Header = note.Header;
            entity.Text = note.Text;
            entity.IsPinned = note.IsPinned;
            entity.IsDeleted = note.IsDeleted;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task DeleteNoteAsync(Guid id)
        {
            await _context.Notes.Where(n => n.Id == id).ExecuteDeleteAsync();
            
            

        }
    }

    
}
