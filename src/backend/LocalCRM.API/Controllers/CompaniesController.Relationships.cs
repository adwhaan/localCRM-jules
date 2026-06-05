using Microsoft.AspNetCore.Mvc;
using LocalCRM.Application.Companies.Commands;

namespace LocalCRM.API.Controllers;

public partial class CompaniesController : ApiControllerBase
{
    [HttpPost("bulk-restore")]
    public async Task<ActionResult> BulkRestore(BulkRestoreCompaniesCommand command)
    {
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpPost("{id}/contacts")]
    public async Task<ActionResult> LinkContact(int id, LinkContactToCompanyCommand command)
    {
        if (id != command.CompanyId) return BadRequest();
        await Mediator.Send(command);
        return NoContent();
    }
}
