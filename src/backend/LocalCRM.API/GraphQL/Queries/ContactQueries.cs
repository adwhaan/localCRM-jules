using HotChocolate;
using LocalCRM.Application.Contacts.Queries;
using LocalCRM.Application.DTOs;
using MediatR;

namespace LocalCRM.API.GraphQL.Queries;

public class ContactQueries
{
    public async Task<List<ContactDto>> GetContacts([Service] IMediator mediator)
    {
        return await mediator.Send(new GetContactsQuery());
    }
}
