using LocalCRM.Application.Interfaces;
using LocalCRM.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace LocalCRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> Get()
    {
        return await Mediator.Send(new LocalCRM.Application.Users.Queries.GetUsersQuery());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var result = await Mediator.Send(new LocalCRM.Application.Users.Queries.GetUserByIdQuery(id));
        if (result == null) return NotFound();
        return result;
    }

    [HttpPost("{id}/disable")]
    public async Task<ActionResult> Disable(int id)
    {
        await Mediator.Send(new LocalCRM.Application.Users.Commands.DisableUserCommand(id));
        return NoContent();
    }
}
