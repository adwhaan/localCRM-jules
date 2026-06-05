using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;
using Microsoft.AspNetCore.Identity;

namespace LocalCRM.Application.Users.Commands;

public record UpdateUserCommand(int Id, string Email, string? Role) : IRequest<UserDto>;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public UpdateUserCommandHandler(UserManager<ApplicationUser> userManager, IMapper mapper, IAuditService audit, ICurrentUserService currentUser)
    {
        _userManager = userManager;
        _mapper = mapper;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());
        if (user == null) throw new Exception("User not found");

        user.Email = request.Email;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded) throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        if (!string.IsNullOrEmpty(request.Role))
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, request.Role);
        }

        await _audit.LogAsync("users", user.Id, "UPDATE", _currentUser.Username ?? "system");

        return _mapper.Map<UserDto>(user);
    }
}
