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

        public async Task<IEnumerable<Note>> GetAllAsync(Guid userId)
        {
            var getAll = await _context.Notes
                .AsNoTracking()
                .Where(n => n.UserId == userId)
                .ToListAsync();
            return getAll;
        }
        public async Task<Note?> GetByIdAsync(Guid id, Guid userId)
        {
            return await _context.Notes.AsNoTracking().SingleOrDefaultAsync(n => n.Id == id && n.UserId == userId);
              
        }
        public async Task<Note> AddNoteAsync(Note note, Guid userId)
        {
            note.UserId = userId;
            _context.Notes.Add(note);
            await _context.SaveChangesAsync();
            return note;
        }
        public async Task<Note> UpdateNoteAsync(Guid id, Note note, Guid userId)
        {
            var entity = await _context.Notes.SingleOrDefaultAsync(n => n.Id == id && n.UserId == userId);

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
        public async Task DeleteNoteAsync(Guid id, Guid userId)
        {
            await _context.Notes.Where(n => n.Id == id && n.UserId == userId).ExecuteDeleteAsync();
            
            

        }
    }

    
}
