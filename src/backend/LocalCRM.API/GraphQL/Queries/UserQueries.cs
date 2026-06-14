using HotChocolate;
using LocalCRM.Application.Users.Queries;
using LocalCRM.Application.DTOs;
using MediatR;
using System.Security.Claims;
using HotChocolate.Authorization;

namespace LocalCRM.API.GraphQL.Queries;

[Authorize]
public class UserQueries
{
    [Authorize(Roles = new[] { "Administrator" })]
    public async Task<List<UserDto>> GetUsers([Service] IMediator mediator)
    {
        return await mediator.Send(new GetUsersQuery());
    }

    [Authorize(Roles = new[] { "Administrator" })]
    public async Task<UserDto?> GetUser(int id, [Service] IMediator mediator)
    {
        return await mediator.Send(new GetUserByIdQuery(id));
    }

    public async Task<UserDto?> GetMe(ClaimsPrincipal principal, [Service] IMediator mediator)
    {
        var idClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (idClaim == null) return null;
        return await mediator.Send(new GetUserByIdQuery(int.Parse(idClaim)));
    }
}
