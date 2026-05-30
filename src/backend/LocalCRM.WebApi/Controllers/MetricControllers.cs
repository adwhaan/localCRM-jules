using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LocalCRM.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<ActionResult<DashboardDto>> Get()
        {
            return Ok(await _dashboardService.GetDashboardDataAsync());
        }
    }

    [ApiController]
    [Route("api/system")]
    [Authorize]
    public class SystemController : ControllerBase
    {
        private readonly ISystemMetricsService _metricsService;

        public SystemController(ISystemMetricsService metricsService)
        {
            _metricsService = metricsService;
        }

        [HttpGet("metrics")]
        public async Task<ActionResult<SystemMetricsDto>> GetMetrics()
        {
            return Ok(await _metricsService.GetSystemMetricsAsync());
        }
    }
}
