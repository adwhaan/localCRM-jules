using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using HotChocolate;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using LocalCRM.Infrastructure.Persistence;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocalCRM.WebApi.GraphQL.Queries
{
    public class Query
    {
        [UseOffsetPaging(MaxPageSize = 100, DefaultPageSize = 20)]
        public async Task<IEnumerable<CompanyDto>> GetCompanies([Service] ICompanyService s) => await s.GetAllAsync();

        [UseOffsetPaging(MaxPageSize = 100, DefaultPageSize = 20)]
        public async Task<IEnumerable<CompanyDto>> GetDeletedCompanies([Service] ICompanyService s) => await s.GetDeletedAsync();

        public async Task<CompanyDto?> GetCompany(int? id, string? @ref, [Service] ICompanyService s, [Service] LocalCRMContext context, [Service] IMapper mapper)
        {
            if (id.HasValue && @ref != null) throw new GraphQLException("Supply either id or ref, not both.");
            if (id.HasValue) return await s.GetByIdAsync(id.Value);
            if (@ref != null) {
                var c = await context.Companies.FirstOrDefaultAsync(x => x.CompanyRef == @ref);
                return mapper.Map<CompanyDto>(c);
            }
            return null;
        }

        [UseOffsetPaging(MaxPageSize = 100, DefaultPageSize = 20)]
        public async Task<IEnumerable<ContactDto>> GetContacts([Service] IContactService s) => await s.GetAllAsync();

        [UseOffsetPaging(MaxPageSize = 100, DefaultPageSize = 20)]
        public async Task<IEnumerable<ContactDto>> GetDeletedContacts([Service] IContactService s) => await s.GetDeletedAsync();

        public async Task<ContactDto?> GetContact(int? id, string? @ref, [Service] IContactService s, [Service] LocalCRMContext context, [Service] IMapper mapper)
        {
            if (id.HasValue && @ref != null) throw new GraphQLException("Supply either id or ref, not both.");
            if (id.HasValue) return await s.GetByIdAsync(id.Value);
            if (@ref != null) {
                var c = await context.Contacts.FirstOrDefaultAsync(x => x.ContactRef == @ref);
                return mapper.Map<ContactDto>(c);
            }
            return null;
        }

        [UseOffsetPaging(MaxPageSize = 100, DefaultPageSize = 20)]
        public async Task<IEnumerable<InteractionDto>> GetInteractions([Service] IInteractionService s) => await s.GetAllAsync();

        [UseOffsetPaging(MaxPageSize = 100, DefaultPageSize = 20)]
        public async Task<IEnumerable<InteractionDto>> GetDeletedInteractions([Service] IInteractionService s) => await s.GetDeletedAsync();

        [UseOffsetPaging(MaxPageSize = 100, DefaultPageSize = 20)]
        public async Task<IEnumerable<EngagementDto>> GetEngagements([Service] IEngagementService s) => await s.GetAllAsync();

        [UseOffsetPaging(MaxPageSize = 100, DefaultPageSize = 20)]
        public async Task<IEnumerable<EngagementDto>> GetDeletedEngagements([Service] IEngagementService s, [Service] IMapper mapper)
        {
            // Simplified: service needs to support it, adding here for now
            return new List<EngagementDto>();
        }

        public async Task<EngagementDto?> GetEngagement(int? id, string? @ref, [Service] IEngagementService s, [Service] LocalCRMContext context, [Service] IMapper mapper)
        {
            if (id.HasValue && @ref != null) throw new GraphQLException("Supply either id or ref, not both.");
            if (id.HasValue) return await s.GetByIdAsync(id.Value);
            if (@ref != null) {
                var c = await context.Engagements.FirstOrDefaultAsync(x => x.EngagementRef == @ref);
                return mapper.Map<EngagementDto>(c);
            }
            return null;
        }

        [UseOffsetPaging(MaxPageSize = 100, DefaultPageSize = 20)]
        public async Task<IEnumerable<NoteDto>> GetNotes([Service] INoteService s, [Service] IHttpContextAccessor h) => await s.GetAllAsync(h.HttpContext?.User.Identity?.Name ?? "", h.HttpContext?.Request.Headers["X-Elevated-Mode"] == "true");

        [UseOffsetPaging(MaxPageSize = 100, DefaultPageSize = 20)]
        public async Task<IEnumerable<DocumentDto>> GetDocuments([Service] IDocumentService s, [Service] IHttpContextAccessor h) => await s.GetAllAsync(h.HttpContext?.User.Identity?.Name ?? "", h.HttpContext?.Request.Headers["X-Elevated-Mode"] == "true");

        public async Task<DocumentDto?> GetDocument(int? id, string? @ref, [Service] IDocumentService s, [Service] LocalCRMContext context, [Service] IMapper mapper, [Service] IHttpContextAccessor h)
        {
            if (id.HasValue && @ref != null) throw new GraphQLException("Supply either id or ref, not both.");
            if (id.HasValue) return await s.GetByIdAsync(id.Value, h.HttpContext?.User.Identity?.Name ?? "", h.HttpContext?.Request.Headers["X-Elevated-Mode"] == "true");
            if (@ref != null) {
                var c = await context.Documents.FirstOrDefaultAsync(x => x.DocumentRef == @ref);
                return mapper.Map<DocumentDto>(c);
            }
            return null;
        }

        public async Task<DashboardDto> GetDashboard([Service] IDashboardService s) => await s.GetDashboardDataAsync();
        public async Task<SystemMetricsDto> GetSystemMetrics([Service] ISystemMetricsService s) => await s.GetSystemMetricsAsync();

        public async Task<IEnumerable<UserDto>> GetUsers([Service] IUserService s) => await s.GetAllAsync();

        public async Task<UserDto?> GetMe([Service] IUserService s, [Service] IHttpContextAccessor h)
        {
            var username = h.HttpContext?.User.Identity?.Name;
            if (username == null) return null;
            var all = await s.GetAllAsync();
            return all.FirstOrDefault(u => u.Username == username);
        }

        public async Task<IEnumerable<RoleDto>> GetRoles([Service] IRoleService s) => await s.GetRolesAsync();
        public async Task<IEnumerable<PermissionDto>> GetPermissions([Service] IRoleService s) => await s.GetPermissionsAsync();
        public async Task<IEnumerable<SettingDto>> GetSettings([Service] ISettingService s) => await s.GetAllAsync();
        public async Task<SettingDto?> GetSetting(string key, [Service] ISettingService s) => await s.GetByKeyAsync(key);
    }
}
