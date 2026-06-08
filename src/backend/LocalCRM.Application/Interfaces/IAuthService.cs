using System.Threading.Tasks;
using LocalCRM.Application.DTOs;

namespace LocalCRM.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(string username, string password);
    Task<AuthResponse?> RefreshTokenAsync(string refreshToken);
    Task<bool> RevokeTokenAsync(string refreshToken);
}
