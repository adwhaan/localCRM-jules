using Microsoft.AspNetCore.Mvc;
using MediatR;
<<<<<<< HEAD

namespace LocalCRM.API.Controllers;

=======
using Microsoft.AspNetCore.Authorization;

namespace LocalCRM.API.Controllers;

[Authorize]
>>>>>>> feature-backend-12855298858282564638
[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private IMediator? _mediator;
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
}
