using HotChocolate;
using LocalCRM.Application.Engagements.Queries;
using LocalCRM.Application.DTOs;
using MediatR;

namespace LocalCRM.API.GraphQL.Queries;

public class EngagementQueries
{
    public async Task<List<EngagementDto>> GetEngagements([Service] IMediator mediator)
    {
        return await mediator.Send(new GetEngagementsQuery());
    }
}
