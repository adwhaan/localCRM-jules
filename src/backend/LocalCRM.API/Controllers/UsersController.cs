using LocalCRM.Application.Interfaces;
using LocalCRM.Application.DTOs;
using LocalCRM.Application.Users.Commands;
using LocalCRM.Application.Users.Queries;
using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD

namespace LocalCRM.API.Controllers;

=======
using Microsoft.AspNetCore.Authorization;

namespace LocalCRM.API.Controllers;

[Authorize(Roles = "Administrator")]
>>>>>>> feature-backend-12855298858282564638
[ApiController]
[Route("api/[controller]")]
public class UsersController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> Get()
    {
        return await Mediator.Send(new GetUsersQuery());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var result = await Mediator.Send(new GetUserByIdQuery(id));
        if (result == null) return NotFound();
        return result;
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(CreateUserCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> Update(int id, UpdateUserCommand command)
    {
        if (id != command.Id) return BadRequest();
        return await Mediator.Send(command);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteUserCommand(id));
        return NoContent();
    }

    [HttpPost("{id}/disable")]
    public async Task<ActionResult> Disable(int id)
    {
        await Mediator.Send(new DisableUserCommand(id));
        return NoContent();
    }
}
