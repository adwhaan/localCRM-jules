using Microsoft.AspNetCore.Mvc;
using LocalCRM.Application.Documents.Queries;
using LocalCRM.Application.Documents.Commands;
using LocalCRM.Application.DTOs;

namespace LocalCRM.API.Controllers;

public class DocumentsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<DocumentDto>>> Get([FromQuery] int offset = 0, [FromQuery] int limit = 10)
    {
        return await Mediator.Send(new GetPagedDocumentsQuery(offset, limit));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DocumentDto>> GetById(int id)
    {
        var result = await Mediator.Send(new GetDocumentByIdQuery(id));
        if (result == null) return NotFound();
        return result;
    }

    [HttpPost]
    public async Task<ActionResult<DocumentDto>> Create(CreateDocumentCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DocumentDto>> Update(int id, UpdateDocumentCommand command)
    {
        // Validation check for ID mismatch if needed
        return await Mediator.Send(command);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await Mediator.Send(new SoftDeleteDocumentCommand(id));
        return NoContent();
    }

    [HttpPost("{id}/restore")]
    public async Task<ActionResult<DocumentDto>> Restore(int id)
    {
        return await Mediator.Send(new RestoreDocumentCommand(id));
    }
}
