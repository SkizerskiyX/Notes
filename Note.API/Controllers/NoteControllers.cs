using Microsoft.AspNetCore.Mvc;
using NoteModels.Dto;
using NotesServices.Interfaces;

namespace Note.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NoteController : ControllerBase
    {
        private readonly INoteService _noteService;
        public NoteController(INoteService noteService)
        {
            _noteService = noteService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var notes = await _noteService.GetAllAsync();
            return Ok(notes);
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetById(Guid id)
        {
            var note = await _noteService.GetByIdAsync(id);
            if (note == null)
                return NotFound();
            return Ok(note);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateNoteDto noteDto)
        {
            var note = new NoteModels.Models.Note(noteDto.Header, noteDto.Text);

            var createdNote = await _noteService.AddNoteAsync(note);
            return CreatedAtAction(nameof(GetById), new { id = createdNote.Id }, createdNote);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, NoteModels.Dto.UpdateNoteDto noteDto)
        {
            try
            { 
                var note = new NoteModels.Models.Note(noteDto.Header, noteDto.Text)
                {
                    IsPinned = noteDto.isPinned
                };
                var updatedNote = await _noteService.UpdateNoteAsync(id, note);
                return Ok(updatedNote);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _noteService.DeleteNoteAsync(id);
            return NoContent();

        }
    }
}
