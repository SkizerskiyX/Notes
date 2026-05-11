using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoteModels.Dto;
using NotesServices.Interfaces;

namespace Note.API.Controllers
{
    [ApiController]
    [Authorize]
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
            var userId = GetUserId();
            var notes = await _noteService.GetAllAsync(userId);
            return Ok(notes);
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetById(Guid id)
        {
            var userId = GetUserId();
            var note = await _noteService.GetByIdAsync(id, userId);
            if (note == null)
                return NotFound();
            return Ok(note);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateNoteDto noteDto)
        {
            var note = new NoteModels.Models.Note(noteDto.Header, noteDto.Text);

            var userId = GetUserId();
            var createdNote = await _noteService.AddNoteAsync(note, userId);
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
                var userId = GetUserId();
                var updatedNote = await _noteService.UpdateNoteAsync(id, note, userId);
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
            var userId = GetUserId();
            await _noteService.DeleteNoteAsync(id, userId);
            return NoContent();

        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user identity.");
            }

            return userId;
        }
    }
}
