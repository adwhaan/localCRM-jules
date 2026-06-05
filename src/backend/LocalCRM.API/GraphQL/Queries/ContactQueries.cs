<<<<<<< HEAD
=======
using HotChocolate.Authorization;
>>>>>>> feature-backend-12855298858282564638
using HotChocolate;
using LocalCRM.Application.Contacts.Queries;
using LocalCRM.Application.DTOs;
using MediatR;

namespace LocalCRM.API.GraphQL.Queries;

<<<<<<< HEAD
=======
[Authorize]
>>>>>>> feature-backend-12855298858282564638
public class ContactQueries
{
    [UseFiltering]
    [UseSorting]
    public async Task<PagedResult<ContactDto>> GetContacts(
        int offset = 0,
        int limit = 10,
        [Service] IMediator mediator = null!)
    {
        return await mediator.Send(new GetPagedContactsQuery(offset, limit));
    }

    public async Task<ContactDto?> GetContact(int id, [Service] IMediator mediator = null!)
    {
        return await mediator.Send(new GetContactByIdQuery(id));
    }

    [UseFiltering]
    [UseSorting]
    public async Task<PagedResult<ContactDto>> GetDeletedContacts(
        int offset = 0,
        int limit = 10,
        [Service] IMediator mediator = null!)
    {
        return await mediator.Send(new GetPagedContactsQuery(offset, limit, true));
    }
}
