using Microsoft.AspNetCore.Mvc;
using LocalCRM.Application.Interfaces;

namespace LocalCRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SettingsController : ApiControllerBase
{
    private readonly IRepository<Domain.Entities.Setting> _repository;

    public SettingsController(IRepository<Domain.Entities.Setting> repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<List<Domain.Entities.Setting>>> Get()
    {
        return await _repository.GetAllAsync();
    }
}
