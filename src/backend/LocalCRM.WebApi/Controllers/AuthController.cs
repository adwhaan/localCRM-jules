using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LocalCRM.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) { _authService = authService; }

        [HttpPost("login")]
        public async Task<ActionResult<AuthPayloadDto>> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto, HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers["User-Agent"]);
            if (result.ErrorCode != null) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthPayloadDto>> Refresh([FromBody] string refreshToken)
        {
            var result = await _authService.RefreshTokenAsync(refreshToken, HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers["User-Agent"]);
            if (result.ErrorCode != null) return BadRequest(result);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            await _authService.RevokeTokenAsync(refreshToken, User.Identity?.Name ?? "");
            return Ok();
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var success = await _authService.ChangePasswordAsync(User.Identity?.Name ?? "", dto.CurrentPassword, dto.NewPassword);
            return success ? Ok() : BadRequest();
        }

        [HttpPost("complete-password-change")]
        public async Task<IActionResult> CompletePasswordChange(CompletePasswordChangeDto dto)
        {
            var success = await _authService.CompletePasswordChangeAsync(dto.Username, dto.NewPassword);
            return success ? Ok() : BadRequest();
        }

        [Authorize]
        [HttpPost("revoke-all-sessions")]
        public async Task<IActionResult> RevokeAllSessions()
        {
            await _authService.RevokeAllSessionsAsync(User.Identity?.Name ?? "");
            return Ok();
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> Me([FromServices] IUserService userService)
        {
            var user = await userService.GetAllAsync();
            var me = user.FirstOrDefault(u => u.Username == User.Identity?.Name);
            return me != null ? Ok(me) : NotFound();
        }
    }

    public class ChangePasswordDto { public string CurrentPassword { get; set; } = ""; public string NewPassword { get; set; } = ""; }
    public class CompletePasswordChangeDto { public string Username { get; set; } = ""; public string NewPassword { get; set; } = ""; }
}
