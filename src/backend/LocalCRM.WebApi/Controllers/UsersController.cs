using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalCRM.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll() => Ok(await _userService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            return user != null ? Ok(user) : NotFound();
        }

        [HttpPost("{id}/disable")]
        public async Task<IActionResult> Disable(int id) => await _userService.UpdateStatusAsync(id, false) ? Ok() : NotFound();

        [HttpPost("{id}/enable")]
        public async Task<IActionResult> Enable(int id) => await _userService.UpdateStatusAsync(id, true) ? Ok() : NotFound();

        [HttpPost("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] string newPassword)
        {
            var success = await _authService.ResetPasswordAsync(id, newPassword, User.Identity?.Name ?? "system");
            return success ? Ok() : NotFound();
        }
    }
}
