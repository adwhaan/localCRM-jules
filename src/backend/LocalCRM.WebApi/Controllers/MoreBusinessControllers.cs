using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using LocalCRM.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
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
        private readonly LocalCRMContext _context;
        private readonly IMapper _mapper;

        public EngagementsController(IEngagementService engagementService, IExportService exportService, LocalCRMContext context, IMapper mapper)
        {
            _engagementService = engagementService;
            _exportService = exportService;
            _context = context;
            _mapper = mapper;
        }

        [HttpGet] public async Task<ActionResult<IEnumerable<EngagementDto>>> GetAll() => Ok(await _engagementService.GetAllAsync());
        [HttpGet("{id}")] public async Task<ActionResult<EngagementDto>> GetById(int id) { var r = await _engagementService.GetByIdAsync(id); return r != null ? Ok(r) : NotFound(); }

        [HttpGet("by-ref/{engagementRef}")]
        public async Task<ActionResult<EngagementDto>> GetByRef(string engagementRef)
        {
            var r = await _context.Engagements.FirstOrDefaultAsync(e => e.EngagementRef == engagementRef);
            return r != null ? Ok(_mapper.Map<EngagementDto>(r)) : NotFound();
        }

        [HttpPost] public async Task<ActionResult<EngagementDto>> Create([FromBody] CreateEngagementDto dto) { var r = await _engagementService.CreateAsync(dto, User.Identity?.Name ?? "system"); return CreatedAtAction(nameof(GetById), new { id = r.EngagementId }, r); }

        [HttpPut("{id}")] public async Task<IActionResult> Update(int id, [FromBody] UpdateEngagementDto dto, [FromHeader(Name = "If-Match")] string? updatedAtStr)
        {
            if (!DateTime.TryParse(updatedAtStr, out var u)) return BadRequest("Missing If-Match");
            return await _engagementService.UpdateAsync(id, dto, User.Identity?.Name ?? "system", u) ? NoContent() : Conflict();
        }

        [HttpPatch("{id}")] public async Task<IActionResult> Patch(int id, [FromBody] UpdateEngagementDto dto, [FromHeader(Name = "If-Match")] string? updatedAtStr) => await Update(id, dto, updatedAtStr);

        [HttpDelete("{id}")] public async Task<IActionResult> Delete(int id) => await _engagementService.DeleteAsync(id, User.Identity?.Name ?? "system") ? NoContent() : NotFound();
        [HttpPost("{id}/restore")] public async Task<IActionResult> Restore(int id) => await _engagementService.RestoreAsync(id, User.Identity?.Name ?? "system") ? Ok() : NotFound();
        [HttpPost("search")] public async Task<ActionResult<IEnumerable<EngagementDto>>> Search([FromQuery] string q) => Ok(await _engagementService.SearchAsync(q));
        [HttpPost("export")] public async Task<IActionResult> Export() { var data = await _engagementService.GetAllAsync(); var csv = await _exportService.ExportToCsvAsync(data); return File(csv, "text/csv", "engagements.csv"); }
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

        [HttpGet] public async Task<ActionResult<IEnumerable<NoteDto>>> GetAll() => Ok(await _noteService.GetAllAsync(User.Identity?.Name ?? "", IsElevated));
        [HttpGet("{id}")] public async Task<ActionResult<NoteDto>> GetById(int id) { var r = await _noteService.GetByIdAsync(id, User.Identity?.Name ?? "", IsElevated); return r != null ? Ok(r) : NotFound(); }
        [HttpPost] public async Task<ActionResult<NoteDto>> Create([FromBody] CreateNoteDto dto) { var r = await _noteService.CreateAsync(dto, User.Identity?.Name ?? "system"); return CreatedAtAction(nameof(GetById), new { id = r.NoteId }, r); }

        [HttpPut("{id}")] public async Task<IActionResult> Update(int id, [FromBody] UpdateNoteDto dto, [FromHeader(Name = "If-Match")] string? updatedAtStr)
        {
            if (!DateTime.TryParse(updatedAtStr, out var u)) return BadRequest("Missing If-Match");
            return await _noteService.UpdateAsync(id, dto, User.Identity?.Name ?? "system", u) ? NoContent() : Conflict();
        }

        [HttpPatch("{id}")] public async Task<IActionResult> Patch(int id, [FromBody] UpdateNoteDto dto, [FromHeader(Name = "If-Match")] string? updatedAtStr) => await Update(id, dto, updatedAtStr);

        [HttpDelete("{id}")] public async Task<IActionResult> Delete(int id) => await _noteService.DeleteAsync(id, User.Identity?.Name ?? "system") ? NoContent() : NotFound();
        [HttpPost("search")] public async Task<ActionResult<IEnumerable<NoteDto>>> Search([FromQuery] string q) => Ok(await _noteService.SearchAsync(q, User.Identity?.Name ?? "", IsElevated));
        [HttpPost("export")] public async Task<IActionResult> Export() { var data = await _noteService.GetAllAsync(User.Identity?.Name ?? "", IsElevated); var csv = await _exportService.ExportToCsvAsync(data); return File(csv, "text/csv", "notes.csv"); }
    }

    [ApiController]
    [Route("api/documents")]
    [Authorize]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly IExportService _exportService;
        private readonly LocalCRMContext _context;
        private readonly IMapper _mapper;

        public DocumentsController(IDocumentService documentService, IExportService exportService, LocalCRMContext context, IMapper mapper)
        {
            _documentService = documentService;
            _exportService = exportService;
            _context = context;
            _mapper = mapper;
        }

        private bool IsElevated => Request.Headers.ContainsKey("X-Elevated-Mode") && Request.Headers["X-Elevated-Mode"] == "true";

        [HttpGet] public async Task<ActionResult<IEnumerable<DocumentDto>>> GetAll() => Ok(await _documentService.GetAllAsync(User.Identity?.Name ?? "", IsElevated));
        [HttpGet("{id}")] public async Task<ActionResult<DocumentDto>> GetById(int id) { var r = await _documentService.GetByIdAsync(id, User.Identity?.Name ?? "", IsElevated); return r != null ? Ok(r) : NotFound(); }

        [HttpGet("by-ref/{documentRef}")]
        public async Task<ActionResult<DocumentDto>> GetByRef(string documentRef)
        {
            var r = await _context.Documents.FirstOrDefaultAsync(d => d.DocumentRef == documentRef);
            if (r == null || (r.Visibility == "priv" && r.CreatedBy != User.Identity?.Name && !IsElevated)) return NotFound();
            return Ok(_mapper.Map<DocumentDto>(r));
        }

        [HttpPost] public async Task<ActionResult<DocumentDto>> Create([FromBody] CreateDocumentDto dto) { var r = await _documentService.CreateAsync(dto, User.Identity?.Name ?? "system"); return CreatedAtAction(nameof(GetById), new { id = r.DocumentId }, r); }

        [HttpPut("{id}")] public async Task<IActionResult> Update(int id, [FromBody] UpdateDocumentDto dto, [FromHeader(Name = "If-Match")] string? updatedAtStr)
        {
            if (!DateTime.TryParse(updatedAtStr, out var u)) return BadRequest("Missing If-Match");
            return await _documentService.UpdateAsync(id, dto, User.Identity?.Name ?? "system", u) ? NoContent() : Conflict();
        }

        [HttpPatch("{id}")] public async Task<IActionResult> Patch(int id, [FromBody] UpdateDocumentDto dto, [FromHeader(Name = "If-Match")] string? updatedAtStr) => await Update(id, dto, updatedAtStr);

        [HttpDelete("{id}")] public async Task<IActionResult> Delete(int id) => await _documentService.DeleteAsync(id, User.Identity?.Name ?? "system") ? NoContent() : NotFound();
        [HttpPost("search")] public async Task<ActionResult<IEnumerable<DocumentDto>>> Search([FromQuery] string q) => Ok(await _documentService.SearchAsync(q, User.Identity?.Name ?? "", IsElevated));
        [HttpPost("export")] public async Task<IActionResult> Export() { var data = await _documentService.GetAllAsync(User.Identity?.Name ?? "", IsElevated); var csv = await _exportService.ExportToCsvAsync(data); return File(csv, "text/csv", "documents.csv"); }
    }
}
