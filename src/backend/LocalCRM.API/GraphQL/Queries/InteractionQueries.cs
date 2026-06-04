using HotChocolate;
using LocalCRM.Application.Interactions.Queries;
using LocalCRM.Application.DTOs;
using MediatR;

namespace LocalCRM.API.GraphQL.Queries;

public class InteractionQueries
{
    public async Task<List<InteractionDto>> GetInteractions([Service] IMediator mediator)
    {
        return await mediator.Send(new GetInteractionsQuery());
    }
}
