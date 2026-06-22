using LocalCRM.Blazor.Models;
using System.Net.Http.Json;

namespace LocalCRM.Blazor.Services;

public class CompanyService
{
    private readonly HttpClient _httpClient;
    public CompanyService(HttpClient httpClient) => _httpClient = httpClient;
    public async Task<PagedResult<CompanyDto>> GetCompaniesAsync(int offset = 0, int limit = 10) =>
        await _httpClient.GetFromJsonAsync<PagedResult<CompanyDto>>($"api/companies?offset={offset}&limit={limit}") ?? new();
}

public class ContactService
{
    private readonly HttpClient _httpClient;
    public ContactService(HttpClient httpClient) => _httpClient = httpClient;
    public async Task<PagedResult<ContactDto>> GetContactsAsync(int offset = 0, int limit = 10) =>
        await _httpClient.GetFromJsonAsync<PagedResult<ContactDto>>($"api/contacts?offset={offset}&limit={limit}") ?? new();
}

public class EngagementService
{
    private readonly HttpClient _httpClient;
    public EngagementService(HttpClient httpClient) => _httpClient = httpClient;
    public async Task<PagedResult<EngagementDto>> GetEngagementsAsync(int offset = 0, int limit = 10) =>
        await _httpClient.GetFromJsonAsync<PagedResult<EngagementDto>>($"api/engagements?offset={offset}&limit={limit}") ?? new();
}

public class InteractionService
{
    private readonly HttpClient _httpClient;
    public InteractionService(HttpClient httpClient) => _httpClient = httpClient;
    public async Task<PagedResult<InteractionDto>> GetInteractionsAsync(int offset = 0, int limit = 10) =>
        await _httpClient.GetFromJsonAsync<PagedResult<InteractionDto>>($"api/interactions?offset={offset}&limit={limit}") ?? new();
}

public class NoteService
{
    private readonly HttpClient _httpClient;
    public NoteService(HttpClient httpClient) => _httpClient = httpClient;
    public async Task<PagedResult<NoteDto>> GetNotesAsync(int offset = 0, int limit = 10) =>
        await _httpClient.GetFromJsonAsync<PagedResult<NoteDto>>($"api/notes?offset={offset}&limit={limit}") ?? new();
}

public class DocumentService
{
    private readonly HttpClient _httpClient;
    public DocumentService(HttpClient httpClient) => _httpClient = httpClient;
    public async Task<PagedResult<DocumentDto>> GetDocumentsAsync(int offset = 0, int limit = 10) =>
        await _httpClient.GetFromJsonAsync<PagedResult<DocumentDto>>($"api/documents?offset={offset}&limit={limit}") ?? new();
}
