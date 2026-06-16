using HotChocolate.Authorization;
using HotChocolate;
using LocalCRM.Application.Contacts.Commands;
using LocalCRM.Application.DTOs;
using MediatR;
using LocalCRM.API.GraphQL.Common;

namespace LocalCRM.API.GraphQL.Mutations;

[Authorize]
public class ContactMutations
{
    public async Task<ContactDto> CreateContact(CreateContactCommand command, [Service] IMediator mediator)
    {
        return await mediator.Send(command);
    }

    public async Task<ContactDto> UpdateContact(UpdateContactCommand command, [Service] IMediator mediator)
    {
        return await mediator.Send(command);
    }

    public async Task<MutationResult> DeleteContact(int id, [Service] IMediator mediator)
    {
        await mediator.Send(new SoftDeleteContactCommand(id));
        return new MutationResult(true, id);
    }

    public async Task<MutationResult> RestoreContact(int id, [Service] IMediator mediator)
    {
        await mediator.Send(new RestoreContactCommand(id));
        return new MutationResult(true, id);
    }

    public async Task<MutationResult> BulkDeleteContacts(List<int> ids, [Service] IMediator mediator)
    {
        await mediator.Send(new BulkDeleteContactsCommand(ids));
        return new MutationResult(true);
    }

    public async Task<MutationResult> BulkRestoreContacts(List<int> ids, [Service] IMediator mediator)
    {
        await mediator.Send(new BulkRestoreContactsCommand(ids));
        return new MutationResult(true);
    }
}
