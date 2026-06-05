<<<<<<< HEAD
=======
using HotChocolate.Authorization;
>>>>>>> feature-backend-12855298858282564638
using HotChocolate;
using LocalCRM.Application.Engagements.Queries;
using LocalCRM.Application.DTOs;
using MediatR;

namespace LocalCRM.API.GraphQL.Queries;

<<<<<<< HEAD
=======
[Authorize]
>>>>>>> feature-backend-12855298858282564638
public class EngagementQueries
{
    [UseFiltering]
    [UseSorting]
    public async Task<PagedResult<EngagementDto>> GetEngagements(
        int offset = 0,
        int limit = 10,
        [Service] IMediator mediator = null!)
    {
        return await mediator.Send(new GetPagedEngagementsQuery(offset, limit));
    }

    public async Task<EngagementDto?> GetEngagement(int id, [Service] IMediator mediator = null!)
    {
        return await mediator.Send(new GetEngagementByIdQuery(id));
    }
}
