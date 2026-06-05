using Microsoft.AspNetCore.Mvc;
using LocalCRM.Application.Interfaces;
<<<<<<< HEAD

namespace LocalCRM.API.Controllers;

=======
using Microsoft.AspNetCore.Authorization;

namespace LocalCRM.API.Controllers;

[Authorize(Roles = "Administrator")]
>>>>>>> feature-backend-12855298858282564638
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
