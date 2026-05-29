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
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompaniesController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAll()
        {
            var results = await _companyService.GetAllAsync();
            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyDto>> GetById(int id)
        {
            var result = await _companyService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<CompanyDto>> Create([FromBody] CreateCompanyDto dto)
        {
            var result = await _companyService.CreateAsync(dto, User.Identity?.Name ?? "system");
            return CreatedAtAction(nameof(GetById), new { id = result.CompanyId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCompanyDto dto, [FromHeader(Name = "If-Match")] string? updatedAtStr)
        {
            if (!DateTime.TryParse(updatedAtStr, out var updatedAt))
                return BadRequest("Missing or invalid If-Match header (UpdatedAt)");

            var success = await _companyService.UpdateAsync(id, dto, User.Identity?.Name ?? "system", updatedAt);
            if (!success) return Conflict(new { code = "concurrency_conflict", message = "The record has been modified by another user." });
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _companyService.DeleteAsync(id, User.Identity?.Name ?? "system");
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpPost("{id}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var success = await _companyService.RestoreAsync(id, User.Identity?.Name ?? "system");
            if (!success) return NotFound();
            return Ok();
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactsController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactDto>>> GetAll()
        {
            var results = await _contactService.GetAllAsync();
            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContactDto>> GetById(int id)
        {
            var result = await _contactService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ContactDto>> Create([FromBody] CreateContactDto dto)
        {
            var result = await _contactService.CreateAsync(dto, User.Identity?.Name ?? "system");
            return CreatedAtAction(nameof(GetById), new { id = result.ContactId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateContactDto dto, [FromHeader(Name = "If-Match")] string? updatedAtStr)
        {
            if (!DateTime.TryParse(updatedAtStr, out var updatedAt))
                return BadRequest("Missing or invalid If-Match header (UpdatedAt)");

            var success = await _contactService.UpdateAsync(id, dto, User.Identity?.Name ?? "system", updatedAt);
            if (!success) return Conflict(new { code = "concurrency_conflict", message = "The record has been modified by another user." });
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _contactService.DeleteAsync(id, User.Identity?.Name ?? "system");
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpPost("{id}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var success = await _contactService.RestoreAsync(id, User.Identity?.Name ?? "system");
            if (!success) return NotFound();
            return Ok();
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InteractionsController : ControllerBase
    {
        private readonly IInteractionService _interactionService;

        public InteractionsController(IInteractionService interactionService)
        {
            _interactionService = interactionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InteractionDto>>> GetAll()
        {
            var results = await _interactionService.GetAllAsync();
            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InteractionDto>> GetById(int id)
        {
            var result = await _interactionService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<InteractionDto>> Create([FromBody] CreateInteractionDto dto)
        {
            var result = await _interactionService.CreateAsync(dto, User.Identity?.Name ?? "system");
            return CreatedAtAction(nameof(GetById), new { id = result.InteractionId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateInteractionDto dto, [FromHeader(Name = "If-Match")] string? updatedAtStr)
        {
            if (!DateTime.TryParse(updatedAtStr, out var updatedAt))
                return BadRequest("Missing or invalid If-Match header (UpdatedAt)");

            var success = await _interactionService.UpdateAsync(id, dto, User.Identity?.Name ?? "system", updatedAt);
            if (!success) return Conflict(new { code = "concurrency_conflict", message = "The record has been modified by another user." });
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _interactionService.DeleteAsync(id, User.Identity?.Name ?? "system");
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpPost("{id}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var success = await _interactionService.RestoreAsync(id, User.Identity?.Name ?? "system");
            if (!success) return NotFound();
            return Ok();
        }
    }
}
