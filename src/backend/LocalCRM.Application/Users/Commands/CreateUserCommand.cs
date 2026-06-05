using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;
using Microsoft.AspNetCore.Identity;

namespace LocalCRM.Application.Users.Commands;

public record CreateUserCommand(string Username, string Email, string Password, string Role) : IRequest<UserDto>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public CreateUserCommandHandler(UserManager<ApplicationUser> userManager, IMapper mapper, IAuditService audit, ICurrentUserService currentUser)
    {
        _userManager = userManager;
        _mapper = mapper;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = request.Username,
            Email = request.Email,
            CreatedBy = _currentUser.Username ?? "system",
            MustChangePassword = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, request.Role);
        await _audit.LogAsync("users", user.Id, "CREATE", user.CreatedBy);

        return _mapper.Map<UserDto>(user);
    }
}
