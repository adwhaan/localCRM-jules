using AutoMapper;
using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Domain.Enums;
using LocalCRM.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        private readonly IRepository<Permission> _permissionRepo;
        private readonly IRepository<RolePermission> _rolePermissionRepo;
        private readonly IRepository<Setting> _settingRepo;
        private readonly IMapper _mapper;
        private readonly IAuditService _auditService;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            IRepository<RefreshToken> tokenRepo,
            IRepository<Permission> permissionRepo,
            IRepository<RolePermission> rolePermissionRepo,
            IRepository<Setting> settingRepo,
            IMapper mapper,
            IAuditService auditService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _tokenRepo = tokenRepo;
            _permissionRepo = permissionRepo;
            _rolePermissionRepo = rolePermissionRepo;
            _settingRepo = settingRepo;
            _mapper = mapper;
            _auditService = auditService;
        }

        public async Task<AuthPayloadDto> LoginAsync(LoginDto loginDto, string? ipAddress, string? userAgent)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user == null || !user.IsActive)
            {
                await _auditService.LogAsync("auth", 0, ActionTypes.LoginFailed, $"Failed login for: {loginDto.Username}", "system");
                return new AuthPayloadDto { ErrorCode = "invalid_credentials" };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, true);
            if (!result.Succeeded)
            {
                await _auditService.LogAsync("auth", user.Id, ActionTypes.LoginFailed, $"Failed password for: {loginDto.Username}", "system");
                return new AuthPayloadDto { ErrorCode = "invalid_credentials" };
            }

            if (user.MustChangePassword) return new AuthPayloadDto { ErrorCode = "password_change_required", User = await MapUserDto(user) };

            return await GenerateAuthPayload(user, ipAddress, userAgent);
        }

        public async Task<AuthPayloadDto> RefreshTokenAsync(string refreshToken, string? ipAddress, string? userAgent)
        {
            var tokenHash = HashToken(refreshToken);
            var tokens = await _tokenRepo.Query().Include(t => t.User).ToListAsync();
            var existingToken = tokens.FirstOrDefault(t => t.TokenHash == tokenHash);

            if (existingToken == null) return new AuthPayloadDto { ErrorCode = "invalid_token" };

            if (existingToken.RevokedAt != null || existingToken.ExpiresAt < DateTime.UtcNow)
            {
                if (existingToken.ReuseDetectedAt == null && existingToken.RevokedAt != null)
                {
                    existingToken.ReuseDetectedAt = DateTime.UtcNow;
                    await _tokenRepo.SaveChangesAsync();
                    await RevokeAllSessionsAsync(existingToken.User?.UserName ?? "");
                }
                return new AuthPayloadDto { ErrorCode = "invalid_token" };
            }

            var user = existingToken.User;
            if (user == null || !user.IsActive) return new AuthPayloadDto { ErrorCode = "invalid_user" };

            // Rotate token
            var newPayload = await GenerateAuthPayload(user, ipAddress, userAgent);

            existingToken.RevokedAt = DateTime.UtcNow;
            await _tokenRepo.SaveChangesAsync();

            return newPayload;
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken, string username)
        {
            var tokenHash = HashToken(refreshToken);
            var existingToken = await _tokenRepo.Query().FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
            if (existingToken == null) return false;
            existingToken.RevokedAt = DateTime.UtcNow;
            await _tokenRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RevokeAllSessionsAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return false;
            var tokens = await _tokenRepo.Query().Where(t => t.UserId == user.Id && t.RevokedAt == null).ToListAsync();
            foreach (var t in tokens) t.RevokedAt = DateTime.UtcNow;
            await _tokenRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePasswordAsync(string username, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return false;
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
            {
                user.MustChangePassword = false;
                await _userManager.UpdateAsync(user);
                await RevokeAllSessionsAsync(username);
                return true;
            }
            return false;
        }

        public async Task<bool> CompletePasswordChangeAsync(string username, string newPassword)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return false;
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (result.Succeeded)
            {
                user.MustChangePassword = false;
                await _userManager.UpdateAsync(user);
                await RevokeAllSessionsAsync(username);
                return true;
            }
            return false;
        }

        public async Task<bool> ResetPasswordAsync(int userId, string newPassword, string adminUsername)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (result.Succeeded)
            {
                user.MustChangePassword = true;
                await _userManager.UpdateAsync(user);
                await RevokeAllSessionsAsync(user.UserName!);
                return true;
            }
            return false;
        }

        private async Task<AuthPayloadDto> GenerateAuthPayload(User user, string? ipAddress, string? userAgent)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var sessionId = Guid.NewGuid().ToString();
            var permissions = await GetUserPermissions(user, roles);
            var accessToken = await GenerateJwtToken(user, roles, permissions, sessionId);
            var refreshTokenString = GenerateRefreshTokenString();

            var refreshTokenLifetime = await GetSettingInt("refresh_token_lifetime", 7 * 24 * 60); // Default 7 days in minutes

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                SessionId = sessionId,
                TokenHash = HashToken(refreshTokenString),
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(refreshTokenLifetime),
                CreatedByIp = ipAddress,
                UserAgent = userAgent
            };

            _tokenRepo.Add(refreshToken);
            await _tokenRepo.SaveChangesAsync();

            return new AuthPayloadDto { AccessToken = accessToken, RefreshToken = refreshTokenString, User = await MapUserDto(user) };
        }

        private async Task<string> GenerateJwtToken(User user, IList<string> roles, IEnumerable<string> permissions, string sessionId)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
                new Claim("preferred_username", user.UserName!),
                new Claim("sid", sessionId)
            };
            foreach (var role in roles) claims.Add(new Claim(ClaimTypes.Role, role));
            foreach (var perm in permissions) claims.Add(new Claim("permissions", perm));

            var jwtKey = _configuration["Jwt:Key"] ?? throw new Exception("JWT Key not configured.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenLifetime = await GetSettingInt("token_lifetime", 60); // Default 60 minutes

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"] ?? "LocalCRM",
                _configuration["Jwt:Audience"] ?? "LocalCRM",
                claims,
                expires: DateTime.UtcNow.AddMinutes(tokenLifetime),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<int> GetSettingInt(string key, int defaultValue)
        {
            var setting = await _settingRepo.Query().FirstOrDefaultAsync(s => s.SettingKey == key);
            if (setting != null && int.TryParse(setting.SettingValue, out var val)) return val;
            return defaultValue;
        }

        private async Task<IEnumerable<string>> GetUserPermissions(User user, IList<string> roles)
        {
            var permissionNames = new HashSet<string>();
            foreach (var roleName in roles)
            {
                var role = await _userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).ThenInclude(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
                    .SelectMany(u => u.UserRoles).Select(ur => ur.Role).FirstOrDefaultAsync(r => r.Name == roleName);
                if (role != null) foreach (var rp in role.RolePermissions) permissionNames.Add(rp.Permission.PermissionName);
            }
            return permissionNames;
        }

        private string GenerateRefreshTokenString() { var b = new byte[32]; using var r = RandomNumberGenerator.Create(); r.GetBytes(b); return Convert.ToBase64String(b); }
        private string HashToken(string t) { using var s = SHA256.Create(); return BitConverter.ToString(s.ComputeHash(Encoding.UTF8.GetBytes(t))).Replace("-", "").ToLower(); }
        private async Task<UserDto> MapUserDto(User user) { var roles = await _userManager.GetRolesAsync(user); return new UserDto { Id = user.Id, Username = user.UserName!, Role = roles.FirstOrDefault() ?? "User" }; }
    }
}
