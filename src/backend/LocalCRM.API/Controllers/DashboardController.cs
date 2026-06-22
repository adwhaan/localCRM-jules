using LocalCRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalCRM.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ApiControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<ActionResult<object>> GetMetrics()
    {
        return await _dashboardService.GetDashboardMetricsAsync();
    }

    [HttpGet("system")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<object>> GetSystemMetrics()
    {
        return await _dashboardService.GetSystemMetricsAsync();
    }
}
