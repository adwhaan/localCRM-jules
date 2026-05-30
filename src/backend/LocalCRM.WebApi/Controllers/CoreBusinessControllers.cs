using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Domain.Enums;
using LocalCRM.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace LocalCRM.WebApi.Controllers
{
    [ApiController]
    [Route("api/companies")]
    [Authorize]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly LocalCRMContext _context;
        private readonly IMapper _mapper;
        private readonly IExportService _exportService;

        public CompaniesController(ICompanyService companyService, LocalCRMContext context, IMapper mapper, IExportService exportService)
        {
            _companyService = companyService;
            _context = context;
            _mapper = mapper;
            _exportService = exportService;
        }

        [HttpGet]
        [Authorize(Policy = Permissions.CompaniesRead)]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAll() => Ok(await _companyService.GetAllAsync());

        [HttpGet("deleted")]
        [Authorize(Policy = Permissions.CompaniesReadDeleted)]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetDeleted() => Ok(await _companyService.GetDeletedAsync());

        [HttpGet("{id}")]
        [Authorize(Policy = Permissions.CompaniesRead)]
        public async Task<ActionResult<CompanyDto>> GetById(int id)
        {
            var result = await _companyService.GetByIdAsync(id);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpGet("by-ref/{companyRef}")]
        [Authorize(Policy = Permissions.CompaniesRead)]
        public async Task<ActionResult<CompanyDto>> GetByRef(string companyRef)
        {
            var result = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyRef == companyRef);
            return result != null ? Ok(_mapper.Map<CompanyDto>(result)) : NotFound();
        }

        [HttpPost]
        [Authorize(Policy = Permissions.CompaniesCreate)]
        public async Task<ActionResult<CompanyDto>> Create([FromBody] CreateCompanyDto dto)
        {
            var result = await _companyService.CreateAsync(dto, User.Identity?.Name ?? "system");
            return CreatedAtAction(nameof(GetById), new { id = result.CompanyId }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = Permissions.CompaniesUpdate)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCompanyDto dto, [FromHeader(Name = "If-Match")] string? updatedAtStr)
        {
            if (!DateTime.TryParse(updatedAtStr, out var updatedAt)) return BadRequest("Missing If-Match");
            return await _companyService.UpdateAsync(id, dto, User.Identity?.Name ?? "system", updatedAt) ? NoContent() : Conflict();
        }

        [HttpPatch("{id}")]
        [Authorize(Policy = Permissions.CompaniesUpdate)]
        public async Task<IActionResult> Patch(int id, [FromBody] UpdateCompanyDto dto, [FromHeader(Name = "If-Match")] string? updatedAtStr) => await Update(id, dto, updatedAtStr);

        [HttpDelete("{id}")]
        [Authorize(Policy = Permissions.CompaniesDelete)]
        public async Task<IActionResult> Delete(int id) => await _companyService.DeleteAsync(id, User.Identity?.Name ?? "system") ? NoContent() : NotFound();

        [HttpPost("{id}/restore")]
        [Authorize(Policy = Permissions.CompaniesRestore)]
        public async Task<IActionResult> Restore(int id) => await _companyService.RestoreAsync(id, User.Identity?.Name ?? "system") ? Ok() : NotFound();

        [HttpPost("search")]
        [Authorize(Policy = Permissions.CompaniesRead)]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> Search([FromQuery] string q) => Ok(await _companyService.SearchAsync(q));

        [HttpPost("bulk-delete")]
        [Authorize(Policy = Permissions.CompaniesDelete)]
        public async Task<ActionResult<int>> BulkDelete([FromBody] IEnumerable<int> ids) => Ok(await _companyService.BulkDeleteAsync(ids, User.Identity?.Name ?? "system"));

        [HttpPost("bulk-restore")]
        [Authorize(Policy = Permissions.CompaniesRestore)]
        public async Task<ActionResult<int>> BulkRestore([FromBody] IEnumerable<int> ids) => Ok(await _companyService.BulkRestoreAsync(ids, User.Identity?.Name ?? "system"));

        [HttpPost("export")]
        [Authorize(Policy = Permissions.CompaniesRead)]
        public async Task<IActionResult> Export()
        {
            var data = await _companyService.GetAllAsync();
            var csv = await _exportService.ExportToCsvAsync(data);
            return File(csv, "text/csv", "companies.csv");
        }
    }

    [ApiController]
    [Route("api/contacts")]
    [Authorize]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _contactService;
        private readonly LocalCRMContext _context;
        private readonly IMapper _mapper;
        private readonly IExportService _exportService;

        public ContactsController(IContactService contactService, LocalCRMContext context, IMapper mapper, IExportService exportService)
        {
            _contactService = contactService;
            _context = context;
            _mapper = mapper;
            _exportService = exportService;
        }

        [HttpGet]
        [Authorize(Policy = Permissions.ContactsRead)]
        public async Task<ActionResult<IEnumerable<ContactDto>>> GetAll() => Ok(await _contactService.GetAllAsync());

        [HttpGet("deleted")]
        [Authorize(Policy = Permissions.ContactsReadDeleted)]
        public async Task<ActionResult<IEnumerable<ContactDto>>> GetDeleted() => Ok(await _contactService.GetDeletedAsync());

        [HttpGet("{id}")]
        [Authorize(Policy = Permissions.ContactsRead)]
        public async Task<ActionResult<ContactDto>> GetById(int id)
        {
            var result = await _contactService.GetByIdAsync(id);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpGet("by-ref/{contactRef}")]
        [Authorize(Policy = Permissions.ContactsRead)]
        public async Task<ActionResult<ContactDto>> GetByRef(string contactRef)
        {
            var result = await _context.Contacts.FirstOrDefaultAsync(c => c.ContactRef == contactRef);
            return result != null ? Ok(_mapper.Map<ContactDto>(result)) : NotFound();
        }

        [HttpPost]
        [Authorize(Policy = Permissions.ContactsCreate)]
        public async Task<ActionResult<ContactDto>> Create([FromBody] CreateContactDto dto)
        {
            var result = await _contactService.CreateAsync(dto, User.Identity?.Name ?? "system");
            return CreatedAtAction(nameof(GetById), new { id = result.ContactId }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = Permissions.ContactsUpdate)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateContactDto dto, [FromHeader(Name = "If-Match")] string? updatedAtStr)
        {
            if (!DateTime.TryParse(updatedAtStr, out var updatedAt)) return BadRequest("Missing If-Match");
            return await _contactService.UpdateAsync(id, dto, User.Identity?.Name ?? "system", updatedAt) ? NoContent() : Conflict();
        }

        [HttpPatch("{id}")]
        [Authorize(Policy = Permissions.ContactsUpdate)]
        public async Task<IActionResult> Patch(int id, [FromBody] UpdateContactDto dto, [FromHeader(Name = "If-Match")] string? updatedAtStr) => await Update(id, dto, updatedAtStr);

        [HttpDelete("{id}")]
        [Authorize(Policy = Permissions.ContactsDelete)]
        public async Task<IActionResult> Delete(int id) => await _contactService.DeleteAsync(id, User.Identity?.Name ?? "system") ? NoContent() : NotFound();

        [HttpPost("{id}/restore")]
        [Authorize(Policy = Permissions.ContactsRestore)]
        public async Task<IActionResult> Restore(int id) => await _contactService.RestoreAsync(id, User.Identity?.Name ?? "system") ? Ok() : NotFound();

        [HttpPost("search")]
        [Authorize(Policy = Permissions.ContactsRead)]
        public async Task<ActionResult<IEnumerable<ContactDto>>> Search([FromQuery] string q) => Ok(await _contactService.SearchAsync(q));

        [HttpPost("export")]
        [Authorize(Policy = Permissions.ContactsRead)]
        public async Task<IActionResult> Export()
        {
            var data = await _contactService.GetAllAsync();
            var csv = await _exportService.ExportToCsvAsync(data);
            return File(csv, "text/csv", "contacts.csv");
        }
    }

    [ApiController]
    [Route("api/interactions")]
    [Authorize]
    public class InteractionsController : ControllerBase
    {
        private readonly IInteractionService _interactionService;
        private readonly IExportService _exportService;
        public InteractionsController(IInteractionService interactionService, IExportService exportService) { _interactionService = interactionService; _exportService = exportService; }

        [HttpGet]
        [Authorize(Policy = Permissions.InteractionsRead)]
        public async Task<ActionResult<IEnumerable<InteractionDto>>> GetAll() => Ok(await _interactionService.GetAllAsync());

        [HttpGet("deleted")]
        [Authorize(Policy = Permissions.InteractionsReadDeleted)]
        public async Task<ActionResult<IEnumerable<InteractionDto>>> GetDeleted() => Ok(await _interactionService.GetDeletedAsync());

        [HttpGet("{id}")]
        [Authorize(Policy = Permissions.InteractionsRead)]
        public async Task<ActionResult<InteractionDto>> GetById(int id) { var r = await _interactionService.GetByIdAsync(id); return r != null ? Ok(r) : NotFound(); }

        [HttpPost]
        [Authorize(Policy = Permissions.InteractionsCreate)]
        public async Task<ActionResult<InteractionDto>> Create([FromBody] CreateInteractionDto dto) { var r = await _interactionService.CreateAsync(dto, User.Identity?.Name ?? "system"); return CreatedAtAction(nameof(GetById), new { id = r.InteractionId }, r); }

        [HttpPut("{id}")]
        [Authorize(Policy = Permissions.InteractionsUpdate)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateInteractionDto dto, [FromHeader(Name = "If-Match")] string? updatedAtStr) { if (!DateTime.TryParse(updatedAtStr, out var u)) return BadRequest("Missing If-Match"); return await _interactionService.UpdateAsync(id, dto, User.Identity?.Name ?? "system", u) ? NoContent() : Conflict(); }

        [HttpPatch("{id}")]
        [Authorize(Policy = Permissions.InteractionsUpdate)]
        public async Task<IActionResult> Patch(int id, [FromBody] UpdateInteractionDto dto, [FromHeader(Name = "If-Match")] string? updatedAtStr) => await Update(id, dto, updatedAtStr);

        [HttpDelete("{id}")]
        [Authorize(Policy = Permissions.InteractionsDelete)]
        public async Task<IActionResult> Delete(int id) => await _interactionService.DeleteAsync(id, User.Identity?.Name ?? "system") ? NoContent() : NotFound();

        [HttpPost("{id}/restore")]
        [Authorize(Policy = Permissions.InteractionsRestore)]
        public async Task<IActionResult> Restore(int id) => await _interactionService.RestoreAsync(id, User.Identity?.Name ?? "system") ? Ok() : NotFound();

        [HttpPost("search")]
        [Authorize(Policy = Permissions.InteractionsRead)]
        public async Task<ActionResult<IEnumerable<InteractionDto>>> Search([FromQuery] string q) => Ok(await _interactionService.SearchAsync(q));

        [HttpPost("export")]
        [Authorize(Policy = Permissions.InteractionsRead)]
        public async Task<IActionResult> Export() { var data = await _interactionService.GetAllAsync(); var csv = await _exportService.ExportToCsvAsync(data); return File(csv, "text/csv", "interactions.csv"); }
    }
}
