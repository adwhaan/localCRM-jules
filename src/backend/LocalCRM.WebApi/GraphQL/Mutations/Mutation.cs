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
        public async Task<int> BulkDeleteCompanies(IEnumerable<int> ids, [Service] ICompanyService s, [Service] IHttpContextAccessor h)
            => await s.BulkDeleteAsync(ids, h.HttpContext?.User.Identity?.Name ?? "system");
        public async Task<int> BulkRestoreCompanies(IEnumerable<int> ids, [Service] ICompanyService s, [Service] IHttpContextAccessor h)
            => await s.BulkRestoreAsync(ids, h.HttpContext?.User.Identity?.Name ?? "system");

        // Contacts
        public async Task<ContactDto> CreateContact(CreateContactDto input, [Service] IContactService s, [Service] IHttpContextAccessor h)
            => await s.CreateAsync(input, h.HttpContext?.User.Identity?.Name ?? "system");
        public async Task<ContactDto?> UpdateContact(int id, UpdateContactDto input, DateTime updatedAt, [Service] IContactService s, [Service] IHttpContextAccessor h)
            => await s.UpdateAsync(id, input, h.HttpContext?.User.Identity?.Name ?? "system", updatedAt) ? await s.GetByIdAsync(id) : null;
        public async Task<MutationResult> DeleteContact(int id, [Service] IContactService s, [Service] IHttpContextAccessor h)
            => await s.DeleteAsync(id, h.HttpContext?.User.Identity?.Name ?? "system") ? MutationResult.SuccessResult(id) : MutationResult.FailureResult();
        public async Task<MutationResult> RestoreContact(int id, [Service] IContactService s, [Service] IHttpContextAccessor h)
            => await s.RestoreAsync(id, h.HttpContext?.User.Identity?.Name ?? "system") ? MutationResult.SuccessResult(id) : MutationResult.FailureResult();
        public async Task<int> BulkDeleteContacts(IEnumerable<int> ids, [Service] IContactService s, [Service] IHttpContextAccessor h)
            => await s.BulkDeleteAsync(ids, h.HttpContext?.User.Identity?.Name ?? "system");
        public async Task<int> BulkRestoreContacts(IEnumerable<int> ids, [Service] IContactService s, [Service] IHttpContextAccessor h)
            => await s.BulkRestoreAsync(ids, h.HttpContext?.User.Identity?.Name ?? "system");

        // Interactions
        public async Task<InteractionDto> CreateInteraction(CreateInteractionDto input, [Service] IInteractionService s, [Service] IHttpContextAccessor h)
            => await s.CreateAsync(input, h.HttpContext?.User.Identity?.Name ?? "system");
        public async Task<InteractionDto?> UpdateInteraction(int id, UpdateInteractionDto input, DateTime updatedAt, [Service] IInteractionService s, [Service] IHttpContextAccessor h)
            => await s.UpdateAsync(id, input, h.HttpContext?.User.Identity?.Name ?? "system", updatedAt) ? await s.GetByIdAsync(id) : null;
        public async Task<MutationResult> DeleteInteraction(int id, [Service] IInteractionService s, [Service] IHttpContextAccessor h)
            => await s.DeleteAsync(id, h.HttpContext?.User.Identity?.Name ?? "system") ? MutationResult.SuccessResult(id) : MutationResult.FailureResult();
        public async Task<MutationResult> RestoreInteraction(int id, [Service] IInteractionService s, [Service] IHttpContextAccessor h)
            => await s.RestoreAsync(id, h.HttpContext?.User.Identity?.Name ?? "system") ? MutationResult.SuccessResult(id) : MutationResult.FailureResult();

        // Engagements
        public async Task<EngagementDto> CreateEngagement(CreateEngagementDto input, [Service] IEngagementService s, [Service] IHttpContextAccessor h)
            => await s.CreateAsync(input, h.HttpContext?.User.Identity?.Name ?? "system");
        public async Task<EngagementDto?> UpdateEngagement(int id, UpdateEngagementDto input, DateTime updatedAt, [Service] IEngagementService s, [Service] IHttpContextAccessor h)
            => await s.UpdateAsync(id, input, h.HttpContext?.User.Identity?.Name ?? "system", updatedAt) ? await s.GetByIdAsync(id) : null;
        public async Task<MutationResult> DeleteEngagement(int id, [Service] IEngagementService s, [Service] IHttpContextAccessor h)
            => await s.DeleteAsync(id, h.HttpContext?.User.Identity?.Name ?? "system") ? MutationResult.SuccessResult(id) : MutationResult.FailureResult();
        public async Task<MutationResult> RestoreEngagement(int id, [Service] IEngagementService s, [Service] IHttpContextAccessor h)
            => await s.RestoreAsync(id, h.HttpContext?.User.Identity?.Name ?? "system") ? MutationResult.SuccessResult(id) : MutationResult.FailureResult();

        // Notes
        public async Task<NoteDto> CreateNote(CreateNoteDto input, [Service] INoteService s, [Service] IHttpContextAccessor h)
            => await s.CreateAsync(input, h.HttpContext?.User.Identity?.Name ?? "system");
        public async Task<NoteDto?> UpdateNote(int id, UpdateNoteDto input, DateTime updatedAt, [Service] INoteService s, [Service] IHttpContextAccessor h)
            => await s.UpdateAsync(id, input, h.HttpContext?.User.Identity?.Name ?? "system", updatedAt) ? await s.GetByIdAsync(id) : null;
        public async Task<MutationResult> DeleteNote(int id, [Service] INoteService s, [Service] IHttpContextAccessor h)
            => await s.DeleteAsync(id, h.HttpContext?.User.Identity?.Name ?? "system") ? MutationResult.SuccessResult(id) : MutationResult.FailureResult();
        public async Task<MutationResult> RestoreNote(int id, [Service] INoteService s, [Service] IHttpContextAccessor h)
            => await s.RestoreAsync(id, h.HttpContext?.User.Identity?.Name ?? "system") ? MutationResult.SuccessResult(id) : MutationResult.FailureResult();

        // Documents
        public async Task<DocumentDto> CreateDocument(CreateDocumentDto input, [Service] IDocumentService s, [Service] IHttpContextAccessor h)
            => await s.CreateAsync(input, h.HttpContext?.User.Identity?.Name ?? "system");
        public async Task<DocumentDto?> UpdateDocument(int id, UpdateDocumentDto input, DateTime updatedAt, [Service] IDocumentService s, [Service] IHttpContextAccessor h)
            => await s.UpdateAsync(id, input, h.HttpContext?.User.Identity?.Name ?? "system", updatedAt) ? await s.GetByIdAsync(id) : null;
        public async Task<MutationResult> DeleteDocument(int id, [Service] IDocumentService s, [Service] IHttpContextAccessor h)
            => await s.DeleteAsync(id, h.HttpContext?.User.Identity?.Name ?? "system") ? MutationResult.SuccessResult(id) : MutationResult.FailureResult();
        public async Task<MutationResult> RestoreDocument(int id, [Service] IDocumentService s, [Service] IHttpContextAccessor h)
            => await s.RestoreAsync(id, h.HttpContext?.User.Identity?.Name ?? "system") ? MutationResult.SuccessResult(id) : MutationResult.FailureResult();

        // Auth
        public async Task<AuthPayloadDto> Login(LoginDto input, [Service] IAuthService s, [Service] IHttpContextAccessor h)
            => await s.LoginAsync(input, h.HttpContext?.Connection.RemoteIpAddress?.ToString(), h.HttpContext?.Request.Headers["User-Agent"]);
        public async Task<AuthPayloadDto> RefreshToken(string refreshToken, [Service] IAuthService s, [Service] IHttpContextAccessor h)
            => await s.RefreshTokenAsync(refreshToken, h.HttpContext?.Connection.RemoteIpAddress?.ToString(), h.HttpContext?.Request.Headers["User-Agent"]);
        public async Task<bool> RevokeToken(string refreshToken, [Service] IAuthService s, [Service] IHttpContextAccessor h)
            => await s.RevokeTokenAsync(refreshToken, h.HttpContext?.User.Identity?.Name ?? "system");
    }
}
