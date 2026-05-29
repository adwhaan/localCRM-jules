using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LocalCRM.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrator")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public UsersController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            return Ok(await _userService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost("{id}/disable")]
        public async Task<IActionResult> Disable(int id)
        {
            var success = await _userService.DisableAsync(id, User.Identity?.Name ?? "system");
            return success ? Ok() : NotFound();
        }

        [HttpPost("{id}/enable")]
        public async Task<IActionResult> Enable(int id)
        {
            var success = await _userService.EnableAsync(id, User.Identity?.Name ?? "system");
            return success ? Ok() : NotFound();
        }

        [HttpPost("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] string newPassword)
        {
            var success = await _authService.ResetPasswordAsync(id, newPassword, User.Identity?.Name ?? "system");
            return success ? Ok() : NotFound();
        }
    }
}
