using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalCRM.WebApi.Controllers
{
    [ApiController]
    [Route("api/engagements")]
    [Authorize]
    public class EngagementsController : ControllerBase
    {
        private readonly IEngagementService _engagementService;
        private readonly IExportService _exportService;

        public EngagementsController(IEngagementService engagementService, IExportService exportService)
        {
            _engagementService = engagementService;
            _exportService = exportService;
        }

        [HttpGet]
        [Authorize(Policy = Permissions.EngagementsRead)]
        public async Task<ActionResult<IEnumerable<EngagementDto>>> GetAll() => Ok(await _engagementService.GetAllAsync());

        [HttpGet("{id}")]
        [Authorize(Policy = Permissions.EngagementsRead)]
        public async Task<ActionResult<EngagementDto>> GetById(int id) { var r = await _engagementService.GetByIdAsync(id); return r != null ? Ok(r) : NotFound(); }

        [HttpPost]
        [Authorize(Policy = Permissions.EngagementsCreate)]
        public async Task<ActionResult<EngagementDto>> Create([FromBody] CreateEngagementDto dto) { var r = await _engagementService.CreateAsync(dto, User.Identity?.Name ?? "system"); return CreatedAtAction(nameof(GetById), new { id = r.EngagementId }, r); }

        [HttpPut("{id}")]
        [Authorize(Policy = Permissions.EngagementsUpdate)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEngagementDto dto, [FromHeader(Name = "If-Match")] string? updatedAtStr)
        {
            if (!DateTime.TryParse(updatedAtStr, out var u)) return BadRequest("Missing If-Match");
            return await _engagementService.UpdateAsync(id, dto, User.Identity?.Name ?? "system", u) ? NoContent() : Conflict();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = Permissions.EngagementsDelete)]
        public async Task<IActionResult> Delete(int id) => await _engagementService.DeleteAsync(id, User.Identity?.Name ?? "system") ? NoContent() : NotFound();

        [HttpPost("{id}/restore")]
        [Authorize(Policy = Permissions.EngagementsRestore)]
        public async Task<IActionResult> Restore(int id) => await _engagementService.RestoreAsync(id, User.Identity?.Name ?? "system") ? Ok() : NotFound();

        [HttpPost("search")]
        [Authorize(Policy = Permissions.EngagementsRead)]
        public async Task<ActionResult<IEnumerable<EngagementDto>>> Search([FromQuery] string q) => Ok(await _engagementService.SearchAsync(q));

        [HttpPost("export")]
        [Authorize(Policy = Permissions.EngagementsRead)]
        public async Task<IActionResult> Export() { var data = await _engagementService.GetAllAsync(); var csv = await _exportService.ExportToCsvAsync(data); return File(csv, "text/csv", "engagements.csv"); }
    }

    [ApiController]
    [Route("api/notes")]
    [Authorize]
    public class NotesController : ControllerBase
    {
        private readonly INoteService _noteService;
        private readonly IExportService _exportService;
        public NotesController(INoteService noteService, IExportService exportService) { _noteService = noteService; _exportService = exportService; }

        private bool IsElevated => Request.Headers.ContainsKey("X-Elevated-Mode") && Request.Headers["X-Elevated-Mode"] == "true";

        [HttpGet]
        [Authorize(Policy = Permissions.NotesRead)]
        public async Task<ActionResult<IEnumerable<NoteDto>>> GetAll() => Ok(await _noteService.GetAllAsync(User.Identity?.Name ?? "", IsElevated));

        [HttpGet("{id}")]
        [Authorize(Policy = Permissions.NotesRead)]
        public async Task<ActionResult<NoteDto>> GetById(int id) { var r = await _noteService.GetByIdAsync(id, User.Identity?.Name ?? "", IsElevated); return r != null ? Ok(r) : NotFound(); }

        [HttpPost]
        [Authorize(Policy = Permissions.NotesCreate)]
        public async Task<ActionResult<NoteDto>> Create([FromBody] CreateNoteDto dto) { var r = await _noteService.CreateAsync(dto, User.Identity?.Name ?? "system"); return CreatedAtAction(nameof(GetById), new { id = r.NoteId }, r); }

        [HttpDelete("{id}")]
        [Authorize(Policy = Permissions.NotesDelete)]
        public async Task<IActionResult> Delete(int id) => await _noteService.DeleteAsync(id, User.Identity?.Name ?? "system") ? NoContent() : NotFound();

        [HttpPost("search")]
        [Authorize(Policy = Permissions.NotesRead)]
        public async Task<ActionResult<IEnumerable<NoteDto>>> Search([FromQuery] string q) => Ok(await _noteService.SearchAsync(q, User.Identity?.Name ?? "", IsElevated));
    }

    [ApiController]
    [Route("api/documents")]
    [Authorize]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly IExportService _exportService;
        public DocumentsController(IDocumentService documentService, IExportService exportService) { _documentService = documentService; _exportService = exportService; }

        private bool IsElevated => Request.Headers.ContainsKey("X-Elevated-Mode") && Request.Headers["X-Elevated-Mode"] == "true";

        [HttpGet]
        [Authorize(Policy = Permissions.DocumentsRead)]
        public async Task<ActionResult<IEnumerable<DocumentDto>>> GetAll() => Ok(await _documentService.GetAllAsync(User.Identity?.Name ?? "", IsElevated));

        [HttpGet("{id}")]
        [Authorize(Policy = Permissions.DocumentsRead)]
        public async Task<ActionResult<DocumentDto>> GetById(int id) { var r = await _documentService.GetByIdAsync(id, User.Identity?.Name ?? "", IsElevated); return r != null ? Ok(r) : NotFound(); }

        [HttpPost]
        [Authorize(Policy = Permissions.DocumentsCreate)]
        public async Task<ActionResult<DocumentDto>> Create([FromBody] CreateDocumentDto dto) { var r = await _documentService.CreateAsync(dto, User.Identity?.Name ?? "system"); return CreatedAtAction(nameof(GetById), new { id = r.DocumentId }, r); }

        [HttpDelete("{id}")]
        [Authorize(Policy = Permissions.DocumentsDelete)]
        public async Task<IActionResult> Delete(int id) => await _documentService.DeleteAsync(id, User.Identity?.Name ?? "system") ? NoContent() : NotFound();

        [HttpPost("search")]
        [Authorize(Policy = Permissions.DocumentsRead)]
        public async Task<ActionResult<IEnumerable<DocumentDto>>> Search([FromQuery] string q) => Ok(await _documentService.SearchAsync(q, User.Identity?.Name ?? "", IsElevated));
    }
}
