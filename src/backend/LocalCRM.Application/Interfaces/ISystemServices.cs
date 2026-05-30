using LocalCRM.Application.DTOs;
using LocalCRM.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalCRM.Application.Interfaces
{
    public interface ITagService
    {
        Task<IEnumerable<TagDto>> GetAllAsync();
        Task<TagDto> CreateAsync(TagDto dto);
        Task<bool> DeleteAsync(int id);
    }

    public interface ISettingService
    {
        Task<IEnumerable<SettingDto>> GetAllAsync();
        Task<SettingDto?> GetByKeyAsync(string key);
        Task<bool> UpdateAsync(string key, string value);
    }

    public interface IAuditLogService
    {
        Task<IEnumerable<AuditLogDto>> GetAllAsync();
        Task<IEnumerable<AuditLogDto>> SearchAsync(string query);
    }

    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetRolesAsync();
        Task<IEnumerable<PermissionDto>> GetPermissionsAsync();
        Task<IEnumerable<PermissionDto>> GetRolePermissionsAsync(int roleId);
        Task<bool> AddPermissionToRoleAsync(int roleId, int permissionId);
        Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId);
    }
}
