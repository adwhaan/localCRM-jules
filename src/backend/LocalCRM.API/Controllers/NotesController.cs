using Microsoft.AspNetCore.Mvc;
using LocalCRM.Application.Notes.Queries;
using LocalCRM.Application.Notes.Commands;
using LocalCRM.Application.DTOs;

namespace LocalCRM.API.Controllers;

public class NotesController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<NoteDto>>> Get([FromQuery] int offset = 0, [FromQuery] int limit = 10)
    {
        return await Mediator.Send(new GetPagedNotesQuery(offset, limit));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NoteDto>> GetById(int id)
    {
        var result = await Mediator.Send(new GetNoteByIdQuery(id));
        if (result == null) return NotFound();
        return result;
    }

    [HttpPost]
    public async Task<ActionResult<NoteDto>> Create(CreateNoteCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<NoteDto>> Update(int id, UpdateNoteCommand command)
    {
        // Validation check for ID mismatch if needed
        return await Mediator.Send(command);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await Mediator.Send(new SoftDeleteNoteCommand(id));
        return NoContent();
    }

    [HttpPost("{id}/restore")]
    public async Task<ActionResult<NoteDto>> Restore(int id)
    {
        return await Mediator.Send(new RestoreNoteCommand(id));
    }
}
