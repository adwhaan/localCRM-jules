using Microsoft.AspNetCore.Mvc;
using LocalCRM.Application.Interactions.Queries;
using LocalCRM.Application.Interactions.Commands;
using LocalCRM.Application.DTOs;

namespace LocalCRM.API.Controllers;

public class InteractionsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<InteractionDto>>> Get([FromQuery] int offset = 0, [FromQuery] int limit = 10)
    {
        return await Mediator.Send(new GetPagedInteractionsQuery(offset, limit));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InteractionDto>> GetById(int id)
    {
        var result = await Mediator.Send(new GetInteractionByIdQuery(id));
        if (result == null) return NotFound();
        return result;
    }

    [HttpPost]
    public async Task<ActionResult<InteractionDto>> Create(CreateInteractionCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<InteractionDto>> Update(int id, UpdateInteractionCommand command)
    {
        // Validation check for ID mismatch if needed
        return await Mediator.Send(command);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await Mediator.Send(new SoftDeleteInteractionCommand(id));
        return NoContent();
    }

    [HttpPost("{id}/restore")]
    public async Task<ActionResult<InteractionDto>> Restore(int id)
    {
        return await Mediator.Send(new RestoreInteractionCommand(id));
    }
}
