using HotChocolate;
using LocalCRM.Application.Users.Queries;
using LocalCRM.Application.DTOs;
using MediatR;
using System.Security.Claims;

namespace LocalCRM.API.GraphQL.Queries;

public class UserQueries
{
    public async Task<List<UserDto>> GetUsers([Service] IMediator mediator)
    {
        return await mediator.Send(new GetUsersQuery());
    }

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
