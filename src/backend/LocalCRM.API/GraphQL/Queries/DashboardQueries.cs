using HotChocolate.Authorization;
using HotChocolate;
using LocalCRM.Application.Interfaces;
using System.Threading.Tasks;

namespace LocalCRM.API.GraphQL.Queries;

[Authorize]
[ExtendObjectType("Query")]
public class DashboardQueries
{
    public async Task<object> GetDashboard([Service] IDashboardService dashboardService)
    {
        return await dashboardService.GetDashboardMetricsAsync();
    }

    [Authorize(Roles = new[] { "Administrator" })]
    public async Task<object> GetSystemMetrics([Service] IDashboardService dashboardService)
    {
        return await dashboardService.GetSystemMetricsAsync();
    }
}
