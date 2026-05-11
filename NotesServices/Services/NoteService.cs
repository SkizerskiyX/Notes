using NoteModels;
using NoteDatabase.Abstraction;
using NotesServices.Interfaces;
using NoteModels.Models;
namespace NotesServices
{
    public class NotesService : INoteService
    {
        private readonly INoteRepository _repository;
        public NotesService(INoteRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<Note>> GetAllAsync(Guid userId)
        {
            return await _repository.GetAllAsync(userId);
        }
        public async Task<Note?> GetByIdAsync(Guid id, Guid userId)
        {
            return await _repository.GetByIdAsync(id, userId);
        }
        public async Task<Note> AddNoteAsync(Note note, Guid userId)
        {
            return await _repository.AddNoteAsync(note, userId);
        }
        public async Task<Note> UpdateNoteAsync(Guid id, Note note, Guid userId)
        {
            return await _repository.UpdateNoteAsync(id, note, userId);
        }
        public async Task DeleteNoteAsync(Guid id, Guid userId)
        {
            await _repository.DeleteNoteAsync(id, userId);
        }
    }
}
