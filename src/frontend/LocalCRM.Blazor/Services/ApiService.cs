using System.Net.Http.Json;
using LocalCRM.Application.DTOs;

namespace LocalCRM.Blazor.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;
        public ApiService(HttpClient http) { _http = http; }

        public async Task<IEnumerable<CompanyDto>?> GetCompaniesAsync() => await _http.GetFromJsonAsync<IEnumerable<CompanyDto>>("api/companies");
        public async Task<CompanyDto?> GetCompanyAsync(int id) => await _http.GetFromJsonAsync<CompanyDto>($"api/companies/{id}");
        public async Task<IEnumerable<ContactDto>?> GetCompanyContactsAsync(int id) => await _http.GetFromJsonAsync<IEnumerable<ContactDto>>($"api/companies/{id}/contacts");
        public async Task<IEnumerable<NoteDto>?> GetCompanyNotesAsync(int id) => await _http.GetFromJsonAsync<IEnumerable<NoteDto>>($"api/companies/{id}/notes");

        public async Task<IEnumerable<ContactDto>?> GetContactsAsync() => await _http.GetFromJsonAsync<IEnumerable<ContactDto>>("api/contacts");
        public async Task<IEnumerable<InteractionDto>?> GetInteractionsAsync() => await _http.GetFromJsonAsync<IEnumerable<InteractionDto>>("api/interactions");
        public async Task<IEnumerable<EngagementDto>?> GetEngagementsAsync() => await _http.GetFromJsonAsync<IEnumerable<EngagementDto>>("api/engagements");
        public async Task<IEnumerable<NoteDto>?> GetNotesAsync() => await _http.GetFromJsonAsync<IEnumerable<NoteDto>>("api/notes");
        public async Task<IEnumerable<DocumentDto>?> GetDocumentsAsync() => await _http.GetFromJsonAsync<IEnumerable<DocumentDto>>("api/documents");
        public async Task<DashboardDto?> GetDashboardAsync() => await _http.GetFromJsonAsync<DashboardDto>("api/dashboard");
        public async Task<IEnumerable<UserDto>?> GetUsersAsync() => await _http.GetFromJsonAsync<IEnumerable<UserDto>>("api/users");
        public async Task<IEnumerable<SettingDto>?> GetSettingsAsync() => await _http.GetFromJsonAsync<IEnumerable<SettingDto>>("api/settings");
        public async Task<IEnumerable<AuditLogDto>?> GetAuditLogsAsync() => await _http.GetFromJsonAsync<IEnumerable<AuditLogDto>>("api/audit-logs");
    }
}
