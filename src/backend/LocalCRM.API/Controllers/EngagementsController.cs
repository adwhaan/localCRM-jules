using Microsoft.AspNetCore.Mvc;
using LocalCRM.Application.Engagements.Queries;
using LocalCRM.Application.Engagements.Commands;
using LocalCRM.Application.DTOs;

namespace LocalCRM.API.Controllers;

public class EngagementsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<EngagementDto>>> Get([FromQuery] int offset = 0, [FromQuery] int limit = 10)
    {
        return await Mediator.Send(new GetPagedEngagementsQuery(offset, limit));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EngagementDto>> GetById(int id)
    {
        var result = await Mediator.Send(new GetEngagementByIdQuery(id));
        if (result == null) return NotFound();
        return result;
    }

    [HttpPost]
    public async Task<ActionResult<EngagementDto>> Create(CreateEngagementCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<EngagementDto>> Update(int id, UpdateEngagementCommand command)
    {
        // Validation check for ID mismatch if needed
        return await Mediator.Send(command);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await Mediator.Send(new SoftDeleteEngagementCommand(EngagementId: id));
        return NoContent();
    }

    [HttpPost("{id}/restore")]
    public async Task<ActionResult<EngagementDto>> Restore(int id)
    {
        return await Mediator.Send(new RestoreEngagementCommand(id));
    }
}
