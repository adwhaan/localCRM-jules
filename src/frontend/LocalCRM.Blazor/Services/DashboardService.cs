using LocalCRM.Blazor.Models;
using System.Net.Http.Json;

namespace LocalCRM.Blazor.Services;

public class DashboardService
{
    private readonly HttpClient _httpClient;

    public DashboardService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DashboardMetrics> GetDashboardMetricsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<DashboardMetrics>("api/dashboard") ?? new();
        }
        catch
        {
            // Fallback for development/offline
            return new DashboardMetrics
            {
                TotalCompanies = 0,
                TotalContacts = 0,
                UpcomingTasks = 0
            };
        }
    }
}
