using Microsoft.AspNetCore.Mvc;
using LocalCRM.Application.Contacts.Queries;
using LocalCRM.Application.Contacts.Commands;
using LocalCRM.Application.DTOs;

namespace LocalCRM.API.Controllers;

public class ContactsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ContactDto>>> Get()
    {
        return await Mediator.Send(new GetContactsQuery());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ContactDto>> GetById(int id)
    {
        var result = await Mediator.Send(new GetContactByIdQuery(id));
        if (result == null) return NotFound();
        return result;
    }

    [HttpPost]
    public async Task<ActionResult<ContactDto>> Create(CreateContactCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ContactDto>> Update(int id, UpdateContactCommand command)
    {
        if (id != command.ContactId) return BadRequest();
        return await Mediator.Send(command);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await Mediator.Send(new SoftDeleteContactCommand(id));
        return NoContent();
    }

    [HttpPost("{id}/restore")]
    public async Task<ActionResult<ContactDto>> Restore(int id)
    {
        return await Mediator.Send(new RestoreContactCommand(id));
    }
}
