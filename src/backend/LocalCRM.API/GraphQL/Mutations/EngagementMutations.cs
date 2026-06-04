using HotChocolate;
using LocalCRM.Application.Engagements.Commands;
using LocalCRM.Application.DTOs;
using MediatR;

namespace LocalCRM.API.GraphQL.Mutations;

public class EngagementMutations
{
    public async Task<EngagementDto> CreateEngagement(CreateEngagementCommand command, [Service] IMediator mediator)
    {
        return await mediator.Send(command);
    }
}
