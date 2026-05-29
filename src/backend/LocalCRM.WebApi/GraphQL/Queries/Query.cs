using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using HotChocolate;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalCRM.WebApi.GraphQL.Queries
{
    public class Query
    {
        public async Task<IEnumerable<CompanyDto>> GetCompanies([Service] ICompanyService s) => await s.GetAllAsync();
        public async Task<CompanyDto?> GetCompany(int id, [Service] ICompanyService s) => await s.GetByIdAsync(id);

        public async Task<IEnumerable<ContactDto>> GetContacts([Service] IContactService s) => await s.GetAllAsync();
        public async Task<ContactDto?> GetContact(int id, [Service] IContactService s) => await s.GetByIdAsync(id);

        public async Task<IEnumerable<InteractionDto>> GetInteractions([Service] IInteractionService s) => await s.GetAllAsync();
        public async Task<InteractionDto?> GetInteraction(int id, [Service] IInteractionService s) => await s.GetByIdAsync(id);

        public async Task<IEnumerable<EngagementDto>> GetEngagements([Service] IEngagementService s) => await s.GetAllAsync();
        public async Task<EngagementDto?> GetEngagement(int id, [Service] IEngagementService s) => await s.GetByIdAsync(id);

        public async Task<IEnumerable<NoteDto>> GetNotes([Service] INoteService s) => await s.GetAllAsync();
        public async Task<NoteDto?> GetNote(int id, [Service] INoteService s) => await s.GetByIdAsync(id);

        public async Task<IEnumerable<DocumentDto>> GetDocuments([Service] IDocumentService s) => await s.GetAllAsync();
        public async Task<DocumentDto?> GetDocument(int id, [Service] IDocumentService s) => await s.GetByIdAsync(id);

        public async Task<DashboardDto> GetDashboard([Service] IDashboardService s) => await s.GetDashboardDataAsync();
        public async Task<SystemMetricsDto> GetSystemMetrics([Service] ISystemMetricsService s) => await s.GetSystemMetricsAsync();

        public async Task<IEnumerable<UserDto>> GetUsers([Service] IUserService s) => await s.GetAllAsync();
        public async Task<UserDto?> GetUser(int id, [Service] IUserService s) => await s.GetByIdAsync(id);

        public async Task<UserDto?> GetMe([Service] IUserService s, [Service] IHttpContextAccessor h)
        {
            var username = h.HttpContext?.User.Identity?.Name;
            if (username == null) return null;
            var all = await s.GetAllAsync();
            return all.FirstOrDefault(u => u.Username == username);
        }
    }
}
