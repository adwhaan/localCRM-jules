using MediatR;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace LocalCRM.Application.Users.Commands;

public record DeleteUserCommand(int Id) : IRequest;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public DeleteUserCommandHandler(UserManager<ApplicationUser> userManager, IAuditService audit, ICurrentUserService currentUser)
    {
        _userManager = userManager;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());
        if (user == null) throw new Exception("User not found");

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded) throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        await _audit.LogAsync("users", user.Id, "DELETE", _currentUser.Username ?? "system");
    }
}
