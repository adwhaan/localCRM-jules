using HotChocolate.Authorization;
using HotChocolate;
using LocalCRM.Application.Interactions.Queries;
using LocalCRM.Application.DTOs;
using MediatR;

namespace LocalCRM.API.GraphQL.Queries;

[Authorize]
public class InteractionQueries
{
    [UseFiltering]
    [UseSorting]
    public async Task<PagedResult<InteractionDto>> GetInteractions(
        int offset = 0,
        int limit = 10,
        [Service] IMediator mediator = null!)
    {
        return await mediator.Send(new GetPagedInteractionsQuery(offset, limit));
    }

    public async Task<InteractionDto?> GetInteraction(int id, [Service] IMediator mediator = null!)
    {
        return await mediator.Send(new GetInteractionByIdQuery(id));
    }

    [UseFiltering]
    [UseSorting]
    public async Task<PagedResult<InteractionDto>> GetDeletedInteractions(
        int offset = 0,
        int limit = 10,
        [Service] IMediator mediator = null!)
    {
        return await mediator.Send(new GetPagedInteractionsQuery(offset, limit, true));
    }
}
