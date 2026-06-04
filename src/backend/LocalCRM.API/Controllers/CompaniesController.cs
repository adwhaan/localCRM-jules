using Microsoft.AspNetCore.Mvc;
using MediatR;
using LocalCRM.Application.Companies.Queries;
using LocalCRM.Application.Companies.Commands;
using LocalCRM.Application.DTOs;

namespace LocalCRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CompaniesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<CompanyDto>>> Get()
    {
        return await _mediator.Send(new GetCompaniesQuery());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CompanyDto>> GetById(int id)
    {
        var result = await _mediator.Send(new GetCompanyByIdQuery(id));
        if (result == null) return NotFound();
        return result;
    }

    [HttpPost]
    public async Task<ActionResult<CompanyDto>> Create(CreateCompanyCommand command)
    {
        return await _mediator.Send(command);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CompanyDto>> Update(int id, UpdateCompanyCommand command)
    {
        if (id != command.CompanyId) return BadRequest();
        return await _mediator.Send(command);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _mediator.Send(new SoftDeleteCompanyCommand(id));
        return NoContent();
    }

    [HttpPost("{id}/restore")]
    public async Task<ActionResult<CompanyDto>> Restore(int id)
    {
        return await _mediator.Send(new RestoreCompanyCommand(id));
    }
}
