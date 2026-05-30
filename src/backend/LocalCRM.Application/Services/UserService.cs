using AutoMapper;
using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Domain.Enums;
using LocalCRM.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocalCRM.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IAuditService _auditService;

        public UserService(UserManager<User> userManager, IMapper mapper, IAuditService auditService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _auditService = auditService;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var dtos = new List<UserDto>();
            foreach (var user in users)
            {
                dtos.Add(await MapUserDto(user));
            }
            return dtos;
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            return user != null ? await MapUserDto(user) : null;
        }

        public async Task<bool> DisableAsync(int id, string adminUsername)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return false;

            user.IsActive = false;
            await _userManager.UpdateAsync(user);
            await _auditService.LogAsync("users", id, ActionTypes.Update, "User disabled", adminUsername);
            return true;
        }

        public async Task<bool> EnableAsync(int id, string adminUsername)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return false;

            user.IsActive = true;
            await _userManager.UpdateAsync(user);
            await _auditService.LogAsync("users", id, ActionTypes.Update, "User enabled", adminUsername);
            return true;
        }

        public async Task<bool> UpdateRoleAsync(int id, string roleName, string adminUsername)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return false;

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, roleName);

            await _auditService.LogAsync("users", id, ActionTypes.Update, $"Role updated to {roleName}", adminUsername);
            return true;
        }

        private async Task<UserDto> MapUserDto(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return new UserDto
            {
                Id = user.Id,
                Username = user.UserName!,
                IsActive = user.IsActive,
                MustChangePassword = user.MustChangePassword,
                Role = roles.FirstOrDefault() ?? "User",
                CreatedAt = user.CreatedAt
            };
        }
    }
}
