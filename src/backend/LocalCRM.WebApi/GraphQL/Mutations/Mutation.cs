using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using LocalCRM.WebApi.GraphQL;
using LocalCRM.Domain.Enums;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalCRM.WebApi.GraphQL.Mutations
{
    public class Mutation
    {
        // Companies
        [Authorize(Policy = Permissions.CompaniesCreate)]
        public async Task<CompanyDto> CreateCompany(CreateCompanyDto input, [Service] ICompanyService s, [Service] IHttpContextAccessor h)
            => await s.CreateAsync(input, h.HttpContext?.User.Identity?.Name ?? "system");

        [Authorize(Policy = Permissions.CompaniesUpdate)]
        public async Task<CompanyDto?> UpdateCompany(int id, UpdateCompanyDto input, DateTime updatedAt, [Service] ICompanyService s, [Service] IHttpContextAccessor h)
        {
            var success = await s.UpdateAsync(id, input, h.HttpContext?.User.Identity?.Name ?? "system", updatedAt);
            if (!success) throw new GraphQLException(ErrorBuilder.New().SetMessage("The record has been modified by another user.").SetCode("concurrency_conflict").Build());
            return await s.GetByIdAsync(id);
        }

        [Authorize(Policy = Permissions.CompaniesDelete)]
        public async Task<MutationResult> DeleteCompany(int id, [Service] ICompanyService s, [Service] IHttpContextAccessor h)
            => await s.DeleteAsync(id, h.HttpContext?.User.Identity?.Name ?? "system") ? MutationResult.SuccessResult(id) : MutationResult.FailureResult();

        [Authorize(Policy = Permissions.CompaniesRestore)]
        public async Task<MutationResult> RestoreCompany(int id, [Service] ICompanyService s, [Service] IHttpContextAccessor h)
            => await s.RestoreAsync(id, h.HttpContext?.User.Identity?.Name ?? "system") ? MutationResult.SuccessResult(id) : MutationResult.FailureResult();

        [Authorize(Policy = Permissions.CompaniesDelete)]
        public async Task<MutationResult> BulkDeleteCompanies(IEnumerable<int> ids, [Service] ICompanyService s, [Service] IHttpContextAccessor h)
            => MutationResult.SuccessResult(await s.BulkDeleteAsync(ids, h.HttpContext?.User.Identity?.Name ?? "system"));

        [Authorize(Policy = Permissions.CompaniesRestore)]
        public async Task<MutationResult> BulkRestoreCompanies(IEnumerable<int> ids, [Service] ICompanyService s, [Service] IHttpContextAccessor h)
            => MutationResult.SuccessResult(await s.BulkRestoreAsync(ids, h.HttpContext?.User.Identity?.Name ?? "system"));

        // Contacts
        [Authorize(Policy = Permissions.ContactsCreate)]
        public async Task<ContactDto> CreateContact(CreateContactDto input, [Service] IContactService s, [Service] IHttpContextAccessor h)
            => await s.CreateAsync(input, h.HttpContext?.User.Identity?.Name ?? "system");

        [Authorize(Policy = Permissions.ContactsUpdate)]
        public async Task<ContactDto?> UpdateContact(int id, UpdateContactDto input, DateTime updatedAt, [Service] IContactService s, [Service] IHttpContextAccessor h)
        {
            var success = await s.UpdateAsync(id, input, h.HttpContext?.User.Identity?.Name ?? "system", updatedAt);
            if (!success) throw new GraphQLException(ErrorBuilder.New().SetMessage("The record has been modified by another user.").SetCode("concurrency_conflict").Build());
            return await s.GetByIdAsync(id);
        }

        [Authorize(Policy = Permissions.ContactsDelete)]
        public async Task<MutationResult> DeleteContact(int id, [Service] IContactService s, [Service] IHttpContextAccessor h)
            => await s.DeleteAsync(id, h.HttpContext?.User.Identity?.Name ?? "system") ? MutationResult.SuccessResult(id) : MutationResult.FailureResult();

        [Authorize(Policy = Permissions.ContactsRestore)]
        public async Task<MutationResult> RestoreContact(int id, [Service] IContactService s, [Service] IHttpContextAccessor h)
            => await s.RestoreAsync(id, h.HttpContext?.User.Identity?.Name ?? "system") ? MutationResult.SuccessResult(id) : MutationResult.FailureResult();

        // User Management
        [Authorize(Policy = Permissions.UsersDisable)]
        public async Task<MutationResult> DisableUser(int id, [Service] IUserService s) => await s.UpdateStatusAsync(id, false) ? MutationResult.SuccessResult(id) : MutationResult.FailureResult();

        [Authorize(Policy = Permissions.UsersDisable)]
        public async Task<MutationResult> EnableUser(int id, [Service] IUserService s) => await s.UpdateStatusAsync(id, true) ? MutationResult.SuccessResult(id) : MutationResult.FailureResult();

        [Authorize(Policy = Permissions.UsersManage)]
        public async Task<MutationResult> ResetUserPassword(int id, string newPassword, [Service] IAuthService auth, [Service] IHttpContextAccessor h)
            => await auth.ResetPasswordAsync(id, newPassword, h.HttpContext?.User.Identity?.Name ?? "system") ? MutationResult.SuccessResult(id) : MutationResult.FailureResult();

        // Settings
        [Authorize(Policy = Permissions.SettingsManage)]
        public async Task<SettingDto?> UpdateSetting(string key, string value, [Service] ISettingService s)
        {
            await s.UpdateAsync(key, value);
            return await s.GetByKeyAsync(key);
        }

        // Roles
        [Authorize(Policy = Permissions.RolesAssignPermissions)]
        public async Task<MutationResult> AssignPermissionToRole(int roleId, int permissionId, [Service] IRoleService s)
            => await s.AddPermissionToRoleAsync(roleId, permissionId) ? MutationResult.SuccessResult(roleId) : MutationResult.FailureResult();

        [Authorize(Policy = Permissions.RolesAssignPermissions)]
        public async Task<MutationResult> RemovePermissionFromRole(int roleId, int permissionId, [Service] IRoleService s)
            => await s.RemovePermissionFromRoleAsync(roleId, permissionId) ? MutationResult.SuccessResult(roleId) : MutationResult.FailureResult();

        // Auth
        public async Task<AuthPayloadDto> Login(LoginDto input, [Service] IAuthService s, [Service] IHttpContextAccessor h)
            => await s.LoginAsync(input, h.HttpContext?.Connection.RemoteIpAddress?.ToString(), h.HttpContext?.Request.Headers["User-Agent"]);
        public async Task<AuthPayloadDto> RefreshToken(string refreshToken, [Service] IAuthService s, [Service] IHttpContextAccessor h)
            => await s.RefreshTokenAsync(refreshToken, h.HttpContext?.Connection.RemoteIpAddress?.ToString(), h.HttpContext?.Request.Headers["User-Agent"]);
        public async Task<MutationResult> Logout(string refreshToken, [Service] IAuthService s, [Service] IHttpContextAccessor h)
            => await s.RevokeTokenAsync(refreshToken, h.HttpContext?.User.Identity?.Name ?? "system") ? MutationResult.SuccessResult(0) : MutationResult.FailureResult();
        public async Task<MutationResult> ChangePassword(ChangePasswordDto input, [Service] IAuthService s, [Service] IHttpContextAccessor h)
            => await s.ChangePasswordAsync(h.HttpContext?.User.Identity?.Name ?? "", input.CurrentPassword, input.NewPassword) ? MutationResult.SuccessResult(0) : MutationResult.FailureResult();
    }
}
