<<<<<<< HEAD
=======
using HotChocolate.Authorization;
>>>>>>> feature-backend-12855298858282564638
using HotChocolate;
using LocalCRM.Application.Interactions.Commands;
using LocalCRM.Application.DTOs;
using MediatR;
using LocalCRM.API.GraphQL.Common;

namespace LocalCRM.API.GraphQL.Mutations;

<<<<<<< HEAD
=======
[Authorize]
>>>>>>> feature-backend-12855298858282564638
public class InteractionMutations
{
    public async Task<InteractionDto> CreateInteraction(CreateInteractionCommand command, [Service] IMediator mediator)
    {
        return await mediator.Send(command);
    }

    public async Task<InteractionDto> UpdateInteraction(UpdateInteractionCommand command, [Service] IMediator mediator)
    {
        return await mediator.Send(command);
    }

    public async Task<MutationResult> DeleteInteraction(int id, [Service] IMediator mediator)
    {
        await mediator.Send(new SoftDeleteInteractionCommand(id));
        return new MutationResult(true, id);
    }

    public async Task<MutationResult> RestoreInteraction(int id, [Service] IMediator mediator)
    {
        await mediator.Send(new RestoreInteractionCommand(id));
        return new MutationResult(true, id);
    }

    public async Task<MutationResult> BulkDeleteInteractions(List<int> ids, [Service] IMediator mediator)
    {
        await mediator.Send(new BulkDeleteInteractionsCommand(ids));
        return new MutationResult(true);
    }

    public async Task<MutationResult> BulkRestoreInteractions(List<int> ids, [Service] IMediator mediator)
    {
        await mediator.Send(new BulkRestoreInteractionsCommand(ids));
        return new MutationResult(true);
    }
}
