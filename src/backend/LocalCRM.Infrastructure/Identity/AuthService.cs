using LocalCRM.Application.Interfaces;
using LocalCRM.Application.DTOs;
using LocalCRM.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using LocalCRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.Extensions.Hosting;

namespace LocalCRM.Infrastructure.Identity;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;
    private readonly IHostEnvironment _environment;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        IMapper mapper,
        ApplicationDbContext context,
        IHostEnvironment environment)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _mapper = mapper;
        _context = context;
        _environment = environment;
    }

    public async Task<AuthResponse?> LoginAsync(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null) return null;

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!result.Succeeded) return null;

        var sessionId = Guid.NewGuid().ToString();
        var accessToken = GenerateJwtToken(user, sessionId);
        var refreshToken = GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = HashToken(refreshToken),
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            SessionId = sessionId
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return new AuthResponse(accessToken, refreshToken, _mapper.Map<UserDto>(user));
    }

    public async Task<AuthResponse?> RefreshTokenAsync(string refreshToken)
    {
        var hashedToken = HashToken(refreshToken);
        var tokenEntity = await _context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == hashedToken);

        if (tokenEntity == null || tokenEntity.RevokedAt != null || tokenEntity.ExpiresAt < DateTime.UtcNow)
        {
            if (tokenEntity?.RevokedAt != null || tokenEntity?.ReuseDetectedAt != null)
            {
                await RevokeAllInSession(tokenEntity?.SessionId);
            }
            return null;
        }

        var newRefreshToken = GenerateRefreshToken();
        var newAccessToken = GenerateJwtToken(tokenEntity.User, tokenEntity.SessionId);

        var newTokenEntity = new RefreshToken
        {
            UserId = tokenEntity.UserId,
            TokenHash = HashToken(newRefreshToken),
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            SessionId = tokenEntity.SessionId
        };

        tokenEntity.RevokedAt = DateTime.UtcNow;

        _context.RefreshTokens.Add(newTokenEntity);
        await _context.SaveChangesAsync();

        tokenEntity.ReplacedByTokenId = newTokenEntity.RefreshTokenId;
        await _context.SaveChangesAsync();

        return new AuthResponse(newAccessToken, newRefreshToken, _mapper.Map<UserDto>(tokenEntity.User));
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        var hashedToken = HashToken(refreshToken);
        var tokenEntity = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hashedToken);
        if (tokenEntity == null) return false;

        tokenEntity.RevokedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task RevokeAllInSession(string? sessionId)
    {
        if (string.IsNullOrEmpty(sessionId)) return;
        var tokens = await _context.RefreshTokens.Where(t => t.SessionId == sessionId && t.RevokedAt == null).ToListAsync();
        foreach (var t in tokens)
        {
            t.RevokedAt = DateTime.UtcNow;
            t.ReuseDetectedAt = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync();
    }

    private string GenerateJwtToken(ApplicationUser user, string sessionId)
    {
        var jwtKey = _configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            if (_environment.IsDevelopment())
            {
                jwtKey = "placeholder_key_at_least_32_characters_long_for_development";
            }
            else
            {
                throw new InvalidOperationException("Jwt:Key is missing from configuration.");
            }
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("sid", sessionId)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
    }
}
