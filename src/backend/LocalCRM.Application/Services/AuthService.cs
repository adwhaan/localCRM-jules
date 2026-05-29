using AutoMapper;
using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Domain.Enums;
using LocalCRM.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LocalCRM.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IRepository<RefreshToken> _tokenRepo;
        private readonly IMapper _mapper;
        private readonly IAuditService _auditService;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            IRepository<RefreshToken> tokenRepo,
            IMapper mapper,
            IAuditService auditService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _tokenRepo = tokenRepo;
            _mapper = mapper;
            _auditService = auditService;
        }

        public async Task<AuthPayloadDto> LoginAsync(LoginDto loginDto, string? ipAddress, string? userAgent)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user == null || !user.IsActive) return new AuthPayloadDto { ErrorCode = "invalid_credentials" };
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, true);
            if (!result.Succeeded) return new AuthPayloadDto { ErrorCode = "invalid_credentials" };
            if (user.MustChangePassword) return new AuthPayloadDto { ErrorCode = "password_change_required", User = await MapUserDto(user) };
            return await GenerateAuthPayload(user, ipAddress, userAgent);
        }

        public async Task<AuthPayloadDto> RefreshTokenAsync(string refreshToken, string? ipAddress, string? userAgent)
        {
            return new AuthPayloadDto { ErrorCode = "not_implemented" };
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken, string username) => false;

        public async Task<bool> ChangePasswordAsync(string username, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return false;
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded) { user.MustChangePassword = false; await _userManager.UpdateAsync(user); return true; }
            return false;
        }

        public async Task<bool> ResetPasswordAsync(int userId, string newPassword, string adminUsername)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (result.Succeeded) { user.MustChangePassword = true; await _userManager.UpdateAsync(user); return true; }
            return false;
        }

        private async Task<AuthPayloadDto> GenerateAuthPayload(User user, string? ipAddress, string? userAgent)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var sessionId = Guid.NewGuid().ToString();
            var accessToken = GenerateJwtToken(user, roles, sessionId);
            return new AuthPayloadDto { AccessToken = accessToken, RefreshToken = "dummy", User = await MapUserDto(user) };
        }

        private string GenerateJwtToken(User user, IList<string> roles, string sessionId)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.UserName!) };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "SUPER_SECRET_KEY_FOR_LOCALCRM_DEVELOPMENT_PURPOSES_LONG_ENOUGH"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken("issuer", "audience", claims, expires: DateTime.UtcNow.AddHours(1), signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<UserDto> MapUserDto(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return new UserDto { Id = user.Id, Username = user.UserName!, Role = roles.FirstOrDefault() ?? "User" };
        }
    }
}
