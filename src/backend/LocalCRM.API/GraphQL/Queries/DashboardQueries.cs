<<<<<<< HEAD
=======
using HotChocolate.Authorization;
>>>>>>> feature-backend-12855298858282564638
using HotChocolate;
using LocalCRM.Application.Interfaces;
using System.Threading.Tasks;

namespace LocalCRM.API.GraphQL.Queries;

<<<<<<< HEAD
=======
[Authorize]
>>>>>>> feature-backend-12855298858282564638
public class DashboardQueries
{
    public async Task<object> GetDashboard([Service] IDashboardService dashboardService)
    {
        return await dashboardService.GetDashboardMetricsAsync();
    }

<<<<<<< HEAD
=======
    [Authorize(Roles = new[] { "Administrator" })]
>>>>>>> feature-backend-12855298858282564638
    public async Task<object> GetSystemMetrics([Service] IDashboardService dashboardService)
    {
        return await dashboardService.GetSystemMetricsAsync();
    }
}
