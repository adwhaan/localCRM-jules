<<<<<<< HEAD
=======
using HotChocolate.Authorization;
>>>>>>> feature-backend-12855298858282564638
using HotChocolate;
using LocalCRM.Application.Engagements.Commands;
using LocalCRM.Application.DTOs;
using MediatR;
using LocalCRM.API.GraphQL.Common;

namespace LocalCRM.API.GraphQL.Mutations;

<<<<<<< HEAD
=======
[Authorize]
>>>>>>> feature-backend-12855298858282564638
public class EngagementMutations
{
    public async Task<EngagementDto> CreateEngagement(CreateEngagementCommand command, [Service] IMediator mediator)
    {
        return await mediator.Send(command);
    }

    public async Task<EngagementDto> UpdateEngagement(UpdateEngagementCommand command, [Service] IMediator mediator)
    {
        return await mediator.Send(command);
    }

    public async Task<MutationResult> DeleteEngagement(int id, [Service] IMediator mediator)
    {
        await mediator.Send(new SoftDeleteEngagementCommand(id));
        return new MutationResult(true, id);
    }

    public async Task<MutationResult> RestoreEngagement(int id, [Service] IMediator mediator)
    {
        await mediator.Send(new RestoreEngagementCommand(id));
        return new MutationResult(true, id);
    }

    public async Task<MutationResult> BulkDeleteEngagements(List<int> ids, [Service] IMediator mediator)
    {
        await mediator.Send(new BulkDeleteEngagementsCommand(ids));
        return new MutationResult(true);
    }

    public async Task<MutationResult> BulkRestoreEngagements(List<int> ids, [Service] IMediator mediator)
    {
        await mediator.Send(new BulkRestoreEngagementsCommand(ids));
        return new MutationResult(true);
    }
}
