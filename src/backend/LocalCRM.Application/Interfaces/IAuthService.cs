using LocalCRM.Application.DTOs;
using System.Threading.Tasks;

namespace LocalCRM.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthPayloadDto> LoginAsync(LoginDto loginDto, string? ipAddress, string? userAgent);
        Task<AuthPayloadDto> RefreshTokenAsync(string refreshToken, string? ipAddress, string? userAgent);
        Task<bool> RevokeTokenAsync(string refreshToken, string username);
        Task<bool> ChangePasswordAsync(string username, string currentPassword, string newPassword);
        Task<bool> ResetPasswordAsync(int userId, string newPassword, string adminUsername);
        Task<bool> CompletePasswordChangeAsync(string username, string newPassword);
        Task<bool> RevokeAllSessionsAsync(string username);
    }
}
