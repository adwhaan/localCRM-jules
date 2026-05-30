using AutoMapper;
using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocalCRM.WebApi.Controllers
{
    [ApiController]
    [Route("api/companies/{id}")]
    [Authorize]
    public class CompanyRelationshipsController : ControllerBase
    {
        private readonly LocalCRMContext _context;
        private readonly IMapper _mapper;
        public CompanyRelationshipsController(LocalCRMContext context, IMapper mapper) { _context = context; _mapper = mapper; }

        [HttpGet("contacts")]
        public async Task<ActionResult<IEnumerable<ContactDto>>> GetContacts(int id)
        {
            var contacts = await _context.CompanyContactLinks.Where(l => l.CompanyId == id).Select(l => l.Contact).ToListAsync();
            return Ok(_mapper.Map<IEnumerable<ContactDto>>(contacts));
        }

        [HttpPost("contacts")]
        public async Task<IActionResult> AddContact(int id, [FromBody] int contactId)
        {
            _context.CompanyContactLinks.Add(new CompanyContactLink { CompanyId = id, ContactId = contactId, CreatedBy = User.Identity?.Name ?? "system", StartDate = DateTime.UtcNow.Date });
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("contacts/{contactId}")]
        public async Task<IActionResult> RemoveContact(int id, int contactId, [FromQuery] DateTime? startDate)
        {
            var query = _context.CompanyContactLinks.Where(l => l.CompanyId == id && l.ContactId == contactId);
            if (startDate.HasValue) query = query.Where(l => l.StartDate == startDate.Value.Date);
            var link = await query.FirstOrDefaultAsync();
            if (link == null) return NotFound();
            link.IsDeleted = true; link.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("notes")] public async Task<ActionResult<IEnumerable<NoteDto>>> GetNotes(int id) { var items = await _context.CompanyNoteLinks.Where(l => l.CompanyId == id).Select(l => l.Note).ToListAsync(); return Ok(_mapper.Map<IEnumerable<NoteDto>>(items)); }
        [HttpPost("notes")] public async Task<IActionResult> AddNote(int id, [FromBody] int noteId) { _context.CompanyNoteLinks.Add(new CompanyNoteLink { CompanyId = id, NoteId = noteId, CreatedBy = User.Identity?.Name ?? "system" }); await _context.SaveChangesAsync(); return Ok(); }
        [HttpDelete("notes/{noteId}")] public async Task<IActionResult> RemoveNote(int id, int noteId) { var link = await _context.CompanyNoteLinks.FirstOrDefaultAsync(l => l.CompanyId == id && l.NoteId == noteId); if (link == null) return NotFound(); link.IsDeleted = true; link.DeletedAt = DateTime.UtcNow; await _context.SaveChangesAsync(); return NoContent(); }

        [HttpGet("documents")] public async Task<ActionResult<IEnumerable<DocumentDto>>> GetDocuments(int id) { var items = await _context.CompanyDocumentLinks.Where(l => l.CompanyId == id).Select(l => l.Document).ToListAsync(); return Ok(_mapper.Map<IEnumerable<DocumentDto>>(items)); }
        [HttpPost("documents")] public async Task<IActionResult> AddDocument(int id, [FromBody] int documentId) { _context.CompanyDocumentLinks.Add(new CompanyDocumentLink { CompanyId = id, DocumentId = documentId, CreatedBy = User.Identity?.Name ?? "system" }); await _context.SaveChangesAsync(); return Ok(); }
        [HttpDelete("documents/{documentId}")] public async Task<IActionResult> RemoveDocument(int id, int documentId) { var link = await _context.CompanyDocumentLinks.FirstOrDefaultAsync(l => l.CompanyId == id && l.DocumentId == documentId); if (link == null) return NotFound(); link.IsDeleted = true; link.DeletedAt = DateTime.UtcNow; await _context.SaveChangesAsync(); return NoContent(); }
    }

    [ApiController]
    [Route("api/contacts/{id}")]
    [Authorize]
    public class ContactRelationshipsController : ControllerBase
    {
        private readonly LocalCRMContext _context;
        private readonly IMapper _mapper;
        public ContactRelationshipsController(LocalCRMContext context, IMapper mapper) { _context = context; _mapper = mapper; }
        [HttpGet("notes")] public async Task<ActionResult<IEnumerable<NoteDto>>> GetNotes(int id) { var items = await _context.ContactNoteLinks.Where(l => l.ContactId == id).Select(l => l.Note).ToListAsync(); return Ok(_mapper.Map<IEnumerable<NoteDto>>(items)); }
        [HttpPost("notes")] public async Task<IActionResult> AddNote(int id, [FromBody] int noteId) { _context.ContactNoteLinks.Add(new ContactNoteLink { ContactId = id, NoteId = noteId, CreatedBy = User.Identity?.Name ?? "system" }); await _context.SaveChangesAsync(); return Ok(); }
        [HttpDelete("notes/{noteId}")] public async Task<IActionResult> RemoveNote(int id, int noteId) { var link = await _context.ContactNoteLinks.FirstOrDefaultAsync(l => l.ContactId == id && l.NoteId == noteId); if (link == null) return NotFound(); link.IsDeleted = true; link.DeletedAt = DateTime.UtcNow; await _context.SaveChangesAsync(); return NoContent(); }
    }

    [ApiController]
    [Route("api/interactions/{id}")]
    [Authorize]
    public class InteractionRelationshipsController : ControllerBase
    {
        private readonly LocalCRMContext _context;
        private readonly IMapper _mapper;
        public InteractionRelationshipsController(LocalCRMContext context, IMapper mapper) { _context = context; _mapper = mapper; }

        [HttpGet("notes")] public async Task<ActionResult<IEnumerable<NoteDto>>> GetNotes(int id) { var items = await _context.InteractionNoteLinks.Where(l => l.InteractionId == id).Select(l => l.Note).ToListAsync(); return Ok(_mapper.Map<IEnumerable<NoteDto>>(items)); }
        [HttpPost("notes")] public async Task<IActionResult> AddNote(int id, [FromBody] int noteId) { _context.InteractionNoteLinks.Add(new InteractionNoteLink { InteractionId = id, NoteId = noteId, CreatedBy = User.Identity?.Name ?? "system" }); await _context.SaveChangesAsync(); return Ok(); }
        [HttpDelete("notes/{noteId}")] public async Task<IActionResult> RemoveNote(int id, int noteId) { var link = await _context.InteractionNoteLinks.FirstOrDefaultAsync(l => l.InteractionId == id && l.NoteId == noteId); if (link == null) return NotFound(); link.IsDeleted = true; link.DeletedAt = DateTime.UtcNow; await _context.SaveChangesAsync(); return NoContent(); }

        [HttpGet("documents")] public async Task<ActionResult<IEnumerable<DocumentDto>>> GetDocuments(int id) { var items = await _context.InteractionDocumentLinks.Where(l => l.InteractionId == id).Select(l => l.Document).ToListAsync(); return Ok(_mapper.Map<IEnumerable<DocumentDto>>(items)); }
        [HttpPost("documents")] public async Task<IActionResult> AddDocument(int id, [FromBody] int documentId) { _context.InteractionDocumentLinks.Add(new InteractionDocumentLink { InteractionId = id, DocumentId = documentId, CreatedBy = User.Identity?.Name ?? "system" }); await _context.SaveChangesAsync(); return Ok(); }
        [HttpDelete("documents/{documentId}")] public async Task<IActionResult> RemoveDocument(int id, int documentId) { var link = await _context.InteractionDocumentLinks.FirstOrDefaultAsync(l => l.InteractionId == id && l.DocumentId == documentId); if (link == null) return NotFound(); link.IsDeleted = true; link.DeletedAt = DateTime.UtcNow; await _context.SaveChangesAsync(); return NoContent(); }

        [HttpPost("link")]
        public async Task<IActionResult> UpdateLink(int id, [FromBody] InteractionLinkDto dto)
        {
            if (dto.ContactId.HasValue && dto.CompanyId.HasValue) return BadRequest("ContactId and CompanyId are mutually exclusive.");
            var link = await _context.InteractionLinks.FirstOrDefaultAsync(l => l.InteractionId == id);
            if (link == null) { link = new InteractionLink { InteractionId = id }; _context.InteractionLinks.Add(link); }
            link.ContactId = dto.ContactId; link.CompanyId = dto.CompanyId; link.EngagementId = dto.EngagementId;
            link.CreatedBy = User.Identity?.Name ?? "system";
            await _context.SaveChangesAsync();
            return Ok();
        }
    }

    public class InteractionLinkDto { public int? ContactId { get; set; } public int? CompanyId { get; set; } public int? EngagementId { get; set; } }

    [ApiController]
    [Route("api/engagements/{id}")]
    [Authorize]
    public class EngagementRelationshipsController : ControllerBase
    {
        private readonly LocalCRMContext _context;
        private readonly IMapper _mapper;
        public EngagementRelationshipsController(LocalCRMContext context, IMapper mapper) { _context = context; _mapper = mapper; }

        [HttpGet("companies")] public async Task<ActionResult<IEnumerable<CompanyDto>>> GetCompanies(int id) { var items = await _context.EngagementCompanyLinks.Where(l => l.EngagementId == id).Select(l => l.Company).ToListAsync(); return Ok(_mapper.Map<IEnumerable<CompanyDto>>(items)); }
        [HttpPost("companies")] public async Task<IActionResult> AddCompany(int id, [FromBody] int companyId) { _context.EngagementCompanyLinks.Add(new EngagementCompanyLink { EngagementId = id, CompanyId = companyId, CreatedBy = User.Identity?.Name ?? "system", StartDate = DateTime.UtcNow.Date }); await _context.SaveChangesAsync(); return Ok(); }
        [HttpDelete("companies/{companyId}")] public async Task<IActionResult> RemoveCompany(int id, int companyId, [FromQuery] DateTime? startDate) { var query = _context.EngagementCompanyLinks.Where(l => l.EngagementId == id && l.CompanyId == companyId); if (startDate.HasValue) query = query.Where(l => l.StartDate == startDate.Value.Date); var link = await query.FirstOrDefaultAsync(); if (link == null) return NotFound(); link.IsDeleted = true; link.DeletedAt = DateTime.UtcNow; await _context.SaveChangesAsync(); return NoContent(); }

        [HttpGet("contacts")] public async Task<ActionResult<IEnumerable<ContactDto>>> GetContacts(int id) { var items = await _context.EngagementContactLinks.Where(l => l.EngagementId == id).Select(l => l.Contact).ToListAsync(); return Ok(_mapper.Map<IEnumerable<ContactDto>>(items)); }
        [HttpPost("contacts")] public async Task<IActionResult> AddContact(int id, [FromBody] int contactId) { _context.EngagementContactLinks.Add(new EngagementContactLink { EngagementId = id, ContactId = contactId, CreatedBy = User.Identity?.Name ?? "system", StartDate = DateTime.UtcNow.Date }); await _context.SaveChangesAsync(); return Ok(); }
        [HttpDelete("contacts/{contactId}")] public async Task<IActionResult> RemoveContact(int id, int contactId, [FromQuery] DateTime? startDate) { var query = _context.EngagementContactLinks.Where(l => l.EngagementId == id && l.ContactId == contactId); if (startDate.HasValue) query = query.Where(l => l.StartDate == startDate.Value.Date); var link = await query.FirstOrDefaultAsync(); if (link == null) return NotFound(); link.IsDeleted = true; link.DeletedAt = DateTime.UtcNow; await _context.SaveChangesAsync(); return NoContent(); }

        [HttpGet("notes")] public async Task<ActionResult<IEnumerable<NoteDto>>> GetNotes(int id) { var items = await _context.EngagementNoteLinks.Where(l => l.EngagementId == id).Select(l => l.Note).ToListAsync(); return Ok(_mapper.Map<IEnumerable<NoteDto>>(items)); }
        [HttpPost("notes")] public async Task<IActionResult> AddNote(int id, [FromBody] int noteId) { _context.EngagementNoteLinks.Add(new EngagementNoteLink { EngagementId = id, NoteId = noteId, CreatedBy = User.Identity?.Name ?? "system" }); await _context.SaveChangesAsync(); return Ok(); }
        [HttpDelete("notes/{noteId}")] public async Task<IActionResult> RemoveNote(int id, int noteId) { var link = await _context.EngagementNoteLinks.FirstOrDefaultAsync(l => l.EngagementId == id && l.NoteId == noteId); if (link == null) return NotFound(); link.IsDeleted = true; link.DeletedAt = DateTime.UtcNow; await _context.SaveChangesAsync(); return NoContent(); }

        [HttpGet("documents")] public async Task<ActionResult<IEnumerable<DocumentDto>>> GetDocuments(int id) { var items = await _context.EngagementDocumentLinks.Where(l => l.EngagementId == id).Select(l => l.Document).ToListAsync(); return Ok(_mapper.Map<IEnumerable<DocumentDto>>(items)); }
        [HttpPost("documents")] public async Task<IActionResult> AddDocument(int id, [FromBody] int documentId) { _context.EngagementDocumentLinks.Add(new EngagementDocumentLink { EngagementId = id, DocumentId = documentId, CreatedBy = User.Identity?.Name ?? "system" }); await _context.SaveChangesAsync(); return Ok(); }
        [HttpDelete("documents/{documentId}")] public async Task<IActionResult> RemoveDocument(int id, int documentId) { var link = await _context.EngagementDocumentLinks.FirstOrDefaultAsync(l => l.EngagementId == id && l.DocumentId == documentId); if (link == null) return NotFound(); link.IsDeleted = true; link.DeletedAt = DateTime.UtcNow; await _context.SaveChangesAsync(); return NoContent(); }
    }
}
