using HotChocolate;
using LocalCRM.Application.Contacts.Commands;
using LocalCRM.Application.DTOs;
using MediatR;

namespace LocalCRM.API.GraphQL.Mutations;

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

    public async Task<bool> DeleteContact(int id, [Service] IMediator mediator)
    {
        await mediator.Send(new SoftDeleteContactCommand(id));
        return true;
    }

    public async Task<ContactDto> RestoreContact(int id, [Service] IMediator mediator)
    {
        return await mediator.Send(new RestoreContactCommand(id));
    }
}
