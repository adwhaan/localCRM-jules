using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalCRM.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EngagementsController : ControllerBase
    {
        private readonly IEngagementService _engagementService;

        public EngagementsController(IEngagementService engagementService)
        {
            _engagementService = engagementService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EngagementDto>>> GetAll()
        {
            var results = await _engagementService.GetAllAsync();
            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EngagementDto>> GetById(int id)
        {
            var result = await _engagementService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<EngagementDto>> Create([FromBody] CreateEngagementDto dto)
        {
            var result = await _engagementService.CreateAsync(dto, User.Identity?.Name ?? "system");
            return CreatedAtAction(nameof(GetById), new { id = result.EngagementId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEngagementDto dto, [FromHeader(Name = "If-Match")] string? updatedAtStr)
        {
            if (!DateTime.TryParse(updatedAtStr, out var updatedAt))
                return BadRequest("Missing or invalid If-Match header (UpdatedAt)");

            var success = await _engagementService.UpdateAsync(id, dto, User.Identity?.Name ?? "system", updatedAt);
            if (!success) return Conflict(new { code = "concurrency_conflict", message = "The record has been modified by another user." });
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _engagementService.DeleteAsync(id, User.Identity?.Name ?? "system");
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpPost("{id}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var success = await _engagementService.RestoreAsync(id, User.Identity?.Name ?? "system");
            if (!success) return NotFound();
            return Ok();
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotesController : ControllerBase
    {
        private readonly INoteService _noteService;

        public NotesController(INoteService noteService)
        {
            _noteService = noteService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NoteDto>>> GetAll()
        {
            var results = await _noteService.GetAllAsync();
            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NoteDto>> GetById(int id)
        {
            var result = await _noteService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<NoteDto>> Create([FromBody] CreateNoteDto dto)
        {
            var result = await _noteService.CreateAsync(dto, User.Identity?.Name ?? "system");
            return CreatedAtAction(nameof(GetById), new { id = result.NoteId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateNoteDto dto, [FromHeader(Name = "If-Match")] string? updatedAtStr)
        {
            if (!DateTime.TryParse(updatedAtStr, out var updatedAt))
                return BadRequest("Missing or invalid If-Match header (UpdatedAt)");

            var success = await _noteService.UpdateAsync(id, dto, User.Identity?.Name ?? "system", updatedAt);
            if (!success) return Conflict(new { code = "concurrency_conflict", message = "The record has been modified by another user." });
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _noteService.DeleteAsync(id, User.Identity?.Name ?? "system");
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpPost("{id}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var success = await _noteService.RestoreAsync(id, User.Identity?.Name ?? "system");
            if (!success) return NotFound();
            return Ok();
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentDto>>> GetAll()
        {
            var results = await _documentService.GetAllAsync();
            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentDto>> GetById(int id)
        {
            var result = await _documentService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<DocumentDto>> Create([FromBody] CreateDocumentDto dto)
        {
            var result = await _documentService.CreateAsync(dto, User.Identity?.Name ?? "system");
            return CreatedAtAction(nameof(GetById), new { id = result.DocumentId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDocumentDto dto, [FromHeader(Name = "If-Match")] string? updatedAtStr)
        {
            if (!DateTime.TryParse(updatedAtStr, out var updatedAt))
                return BadRequest("Missing or invalid If-Match header (UpdatedAt)");

            var success = await _documentService.UpdateAsync(id, dto, User.Identity?.Name ?? "system", updatedAt);
            if (!success) return Conflict(new { code = "concurrency_conflict", message = "The record has been modified by another user." });
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _documentService.DeleteAsync(id, User.Identity?.Name ?? "system");
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpPost("{id}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var success = await _documentService.RestoreAsync(id, User.Identity?.Name ?? "system");
            if (!success) return NotFound();
            return Ok();
        }
    }
}
