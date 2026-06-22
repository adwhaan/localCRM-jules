using LocalCRM.Blazor.Models;
using System.Net.Http.Json;

namespace LocalCRM.Blazor.Services;

public class UserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<UserDto>>("api/users") ?? new();
    }

    public async Task<UserDto?> CreateUserAsync(string username, string email, string password, string role)
    {
        var response = await _httpClient.PostAsJsonAsync("api/users", new { Username = username, Email = email, Password = password, Role = role });
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<UserDto>();
        }
        return null;
    }

    public async Task UpdateUserAsync(int id, string email, string role)
    {
        await _httpClient.PutAsJsonAsync($"api/users/{id}", new { Id = id, Email = email, Role = role });
    }

    public async Task DisableUserAsync(int id)
    {
        await _httpClient.PostAsync($"api/users/{id}/disable", null);
    }

    public async Task DeleteUserAsync(int id)
    {
        await _httpClient.DeleteAsync($"api/users/{id}");
    }
}

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
