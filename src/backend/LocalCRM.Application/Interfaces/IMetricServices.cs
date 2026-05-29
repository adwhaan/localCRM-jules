using LocalCRM.Application.DTOs;
using System.Threading.Tasks;

namespace LocalCRM.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardDataAsync();
    }

    public interface ISystemMetricsService
    {
        Task<SystemMetricsDto> GetSystemMetricsAsync();
        void IncrementApiCall();
        void IncrementApiFailure();
        void UpdateLastAction();
    }
}
