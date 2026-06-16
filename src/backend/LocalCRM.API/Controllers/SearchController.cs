using Microsoft.AspNetCore.Mvc;
using LocalCRM.Application.Search.Queries;
using LocalCRM.Application.DTOs;

namespace LocalCRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<SearchResultDto>>> Search([FromQuery] string term)
    {
        return await Mediator.Send(new GlobalSearchQuery(term));
    }
}
