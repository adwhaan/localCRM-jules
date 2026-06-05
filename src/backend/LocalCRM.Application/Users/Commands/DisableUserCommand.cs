using MediatR;
using LocalCRM.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using LocalCRM.Domain.Entities;

namespace LocalCRM.Application.Users.Commands;

public record DisableUserCommand(int Id) : IRequest;

public class DisableUserCommandHandler : IRequestHandler<DisableUserCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public DisableUserCommandHandler(UserManager<ApplicationUser> userManager, IAuditService audit, ICurrentUserService currentUser)
    {
        _userManager = userManager;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task Handle(DisableUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());
        if (user == null) return;

        await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
        await _audit.LogAsync("users", user.Id, "DISABLE", _currentUser.Username ?? "system");
    }
}
