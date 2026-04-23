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
        public async Task<IEnumerable<Note>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<Note?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }
        public async Task<Note> AddNoteAsync(Note note)
        {
            return await _repository.AddNoteAsync(note);
        }
        public async Task<Note> UpdateNoteAsync(Guid id, Note note)
        {
            return await _repository.UpdateNoteAsync(id, note);
        }
        public async Task DeleteNoteAsync(Guid id)
        {
            await _repository.DeleteNoteAsync(id);
        }
    }
}
