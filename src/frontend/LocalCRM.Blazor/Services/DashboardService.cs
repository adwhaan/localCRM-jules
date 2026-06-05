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
        // For now, using a mock until the API is fully wired and CORS is configured
        // In a real scenario, this would be:
        // return await _httpClient.GetFromJsonAsync<DashboardMetrics>("api/dashboard") ?? new();

        await Task.Delay(500); // Simulate network latency
        return new DashboardMetrics
        {
            TotalCompanies = 124,
            TotalContacts = 456,
            UpcomingTasks = 8
        };
    }
}
