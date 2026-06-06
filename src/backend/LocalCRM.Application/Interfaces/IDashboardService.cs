using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalCRM.Application.Interfaces;

public interface IDashboardService
{
    Task<object> GetDashboardMetricsAsync();
    Task<object> GetSystemMetricsAsync();
}
