using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using LocalCRM.Blazor.Models;

namespace LocalCRM.Blazor.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly ILocalStorageService _localStorage;

    public AuthService(HttpClient httpClient,
                       AuthenticationStateProvider authenticationStateProvider,
                       ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _authenticationStateProvider = authenticationStateProvider;
        _localStorage = localStorage;
    }

    public async Task<AuthResult> Login(string username, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { Username = username, Password = password });

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                 var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                 if (error?.Code == "password_change_required") return AuthResult.PasswordChangeRequired();
            }
            return AuthResult.Failure();
        }

        var result = await response.Content.ReadFromJsonAsync<LoginResult>();

        if (result == null || string.IsNullOrEmpty(result.AccessToken))
        {
            return AuthResult.Failure();
        }

        await _localStorage.SetItemAsync("authToken", result.AccessToken);
        await _localStorage.SetItemAsync("refreshToken", result.RefreshToken);

        ((JwtAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(result.AccessToken);

        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", result.AccessToken);

        return AuthResult.Success();
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("authToken");
        await _localStorage.RemoveItemAsync("refreshToken");
        ((JwtAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }
}

public class AuthResult
{
    public bool Succeeded { get; set; }
    public bool MustChangePassword { get; set; }

    public static AuthResult Success() => new() { Succeeded = true };
    public static AuthResult Failure() => new() { Succeeded = false };
    public static AuthResult PasswordChangeRequired() => new() { Succeeded = false, MustChangePassword = true };
}

public class LoginResult
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

public class ErrorResponse
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
