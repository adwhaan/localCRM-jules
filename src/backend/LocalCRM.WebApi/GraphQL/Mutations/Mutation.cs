using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using LocalCRM.WebApi.GraphQL;
using HotChocolate;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalCRM.WebApi.GraphQL.Mutations
{
    public class Mutation
    {
        // Companies
        public async Task<CompanyDto> CreateCompany(CreateCompanyDto input, [Service] ICompanyService s, [Service] IHttpContextAccessor h)
            => await s.CreateAsync(input, h.HttpContext?.User.Identity?.Name ?? "system");
        public async Task<CompanyDto?> UpdateCompany(int id, UpdateCompanyDto input, DateTime updatedAt, [Service] ICompanyService s, [Service] IHttpContextAccessor h)
            => await s.UpdateAsync(id, input, h.HttpContext?.User.Identity?.Name ?? "system", updatedAt) ? await s.GetByIdAsync(id) : null;
        public async Task<MutationResult> DeleteCompany(int id, [Service] ICompanyService s, [Service] IHttpContextAccessor h)
            => await s.DeleteAsync(id, h.HttpContext?.User.Identity?.Name ?? "system") ? MutationResult.SuccessResult(id) : MutationResult.FailureResult();
        public async Task<MutationResult> RestoreCompany(int id, [Service] ICompanyService s, [Service] IHttpContextAccessor h)
            => await s.RestoreAsync(id, h.HttpContext?.User.Identity?.Name ?? "system") ? MutationResult.SuccessResult(id) : MutationResult.FailureResult();

        // Contacts
        public async Task<ContactDto> CreateContact(CreateContactDto input, [Service] IContactService s, [Service] IHttpContextAccessor h)
            => await s.CreateAsync(input, h.HttpContext?.User.Identity?.Name ?? "system");
        public async Task<ContactDto?> UpdateContact(int id, UpdateContactDto input, DateTime updatedAt, [Service] IContactService s, [Service] IHttpContextAccessor h)
            => await s.UpdateAsync(id, input, h.HttpContext?.User.Identity?.Name ?? "system", updatedAt) ? await s.GetByIdAsync(id) : null;
        public async Task<MutationResult> DeleteContact(int id, [Service] IContactService s, [Service] IHttpContextAccessor h)
            => await s.DeleteAsync(id, h.HttpContext?.User.Identity?.Name ?? "system") ? MutationResult.SuccessResult(id) : MutationResult.FailureResult();
        public async Task<MutationResult> RestoreContact(int id, [Service] IContactService s, [Service] IHttpContextAccessor h)
            => await s.RestoreAsync(id, h.HttpContext?.User.Identity?.Name ?? "system") ? MutationResult.SuccessResult(id) : MutationResult.FailureResult();

        // Interactions
        public async Task<InteractionDto> CreateInteraction(CreateInteractionDto input, [Service] IInteractionService s, [Service] IHttpContextAccessor h)
            => await s.CreateAsync(input, h.HttpContext?.User.Identity?.Name ?? "system");

        // Engagements
        public async Task<EngagementDto> CreateEngagement(CreateEngagementDto input, [Service] IEngagementService s, [Service] IHttpContextAccessor h)
            => await s.CreateAsync(input, h.HttpContext?.User.Identity?.Name ?? "system");

        // Notes
        public async Task<NoteDto> CreateNote(CreateNoteDto input, [Service] INoteService s, [Service] IHttpContextAccessor h)
            => await s.CreateAsync(input, h.HttpContext?.User.Identity?.Name ?? "system");

        // Documents
        public async Task<DocumentDto> CreateDocument(CreateDocumentDto input, [Service] IDocumentService s, [Service] IHttpContextAccessor h)
            => await s.CreateAsync(input, h.HttpContext?.User.Identity?.Name ?? "system");

        // Auth
        public async Task<AuthPayloadDto> Login(LoginDto input, [Service] IAuthService s, [Service] IHttpContextAccessor h)
            => await s.LoginAsync(input, h.HttpContext?.Connection.RemoteIpAddress?.ToString(), h.HttpContext?.Request.Headers["User-Agent"]);
    }
}
