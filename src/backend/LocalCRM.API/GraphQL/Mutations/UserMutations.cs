using HotChocolate;
using LocalCRM.Application.Users.Commands;
using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using MediatR;
using LocalCRM.API.GraphQL.Common;
using LocalCRM.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using HotChocolate.Authorization;

[Authorize]
public class UserMutations
{
    [Authorize(Roles = new[] { "Administrator" })]
    public async Task<UserDto> CreateUser(CreateUserCommand command, [Service] IMediator mediator)
    {
        return await mediator.Send(command);
    }

    [Authorize(Roles = new[] { "Administrator" })]
    public async Task<UserDto> UpdateUser(UpdateUserCommand command, [Service] IMediator mediator)
    {
        return await mediator.Send(command);
    }

    [Authorize(Roles = new[] { "Administrator" })]
    public async Task<MutationResult> DeleteUser(int id, [Service] IMediator mediator)
    {
        await mediator.Send(new DeleteUserCommand(id));
        return new MutationResult(true, id);
    }

    [Authorize(Roles = new[] { "Administrator" })]
    public async Task<MutationResult> DisableUser(int id, [Service] IMediator mediator)
    {
        await mediator.Send(new DisableUserCommand(id));
        return new MutationResult(true, id);
    }

    [Authorize(Roles = new[] { "Administrator" })]
    public async Task<MutationResult> EnableUser(int id, [Service] UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user != null)
        {
            await userManager.SetLockoutEndDateAsync(user, null);
            return new MutationResult(true, id);
        }
        return new MutationResult(false);
    }
}

public record LoginRequest(string Username, string Password);

public class AuthMutations
{
    public async Task<AuthResponse?> Login(LoginRequest input, [Service] IAuthService authService)
    {
        return await authService.LoginAsync(input.Username, input.Password);
    }

    public async Task<AuthResponse?> RefreshToken(string refreshToken, [Service] IAuthService authService)
    {
        return await authService.RefreshTokenAsync(refreshToken);
    }

    [Authorize]
    public async Task<MutationResult> Logout(string refreshToken, [Service] IAuthService authService)
    {
        await authService.RevokeTokenAsync(refreshToken);
        return new MutationResult(true);
    }
}
