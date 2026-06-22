using Microsoft.AspNetCore.Mvc;
using MediatR;
using LocalCRM.Application.Companies.Queries;
using LocalCRM.Application.Companies.Commands;
using LocalCRM.Application.DTOs;

namespace LocalCRM.API.Controllers;

public partial class CompaniesController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<CompanyDto>>> Get([FromQuery] int offset = 0, [FromQuery] int limit = 10)
    {
        return await Mediator.Send(new GetPagedCompaniesQuery(offset, limit));
    }

    [HttpGet("deleted")]
    public async Task<ActionResult<PagedResult<CompanyDto>>> GetDeleted([FromQuery] int offset = 0, [FromQuery] int limit = 10)
    {
        return await Mediator.Send(new GetPagedCompaniesQuery(offset, limit, true));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CompanyDto>> GetById(int id)
    {
        var result = await Mediator.Send(new GetCompanyByIdQuery(id));
        if (result == null) return NotFound();
        return result;
    }

    [HttpGet("by-ref/{companyRef}")]
    public async Task<ActionResult<CompanyDto>> GetByRef(string companyRef)
    {
        var result = await Mediator.Send(new GetCompanyByRefQuery(companyRef));
        if (result == null) return NotFound();
        return result;
    }

    [HttpPost]
    public async Task<ActionResult<CompanyDto>> Create(CreateCompanyCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CompanyDto>> Update(int id, UpdateCompanyCommand command)
    {
        if (id != command.CompanyId) return BadRequest();
        return await Mediator.Send(command);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await Mediator.Send(new SoftDeleteCompanyCommand(CompanyId: id));
        return NoContent();
    }

    [HttpPost("{id}/restore")]
    public async Task<ActionResult<CompanyDto>> Restore(int id)
    {
        return await Mediator.Send(new RestoreCompanyCommand(id));
    }

    [HttpPost("bulk-delete")]
    public async Task<ActionResult> BulkDelete(BulkDeleteCompaniesCommand command)
    {
        await Mediator.Send(command);
        return NoContent();
    }
}
