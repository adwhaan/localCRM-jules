using HotChocolate.Authorization;
using HotChocolate;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using LocalCRM.Application.DTOs;

namespace LocalCRM.API.GraphQL.Queries;

[Authorize]
public class RoleQueries
{
    public async Task<List<ApplicationRole>> GetRoles([Service] IRepository<ApplicationRole> repository)
    {
        return await repository.GetAllAsync();
    }
}

[Authorize]
public class PermissionQueries
{
    public async Task<List<Permission>> GetPermissions([Service] IRepository<Permission> repository)
    {
        return await repository.GetAllAsync();
    }
}

[Authorize]
public class SettingQueries
{
    public async Task<List<Setting>> GetSettings([Service] IRepository<Setting> repository)
    {
        return await repository.GetAllAsync();
    }

    public async Task<Setting?> GetSetting(string key, [Service] IRepository<Setting> repository)
    {
        return await repository.Query().FirstOrDefaultAsync(s => s.Key == key);
    }
}

[Authorize]
public class SystemQueries
{
    public async Task<PagedResult<AuditLog>> GetAuditLogs(
        int offset = 0,
        int limit = 10,
        [Service] IRepository<AuditLog> repository = null!)
    {
        var query = repository.Query().OrderByDescending(a => a.PerformedAt);
        var totalCount = await query.CountAsync();
        var items = await query.Skip(offset).Take(limit).ToListAsync();

        return new PagedResult<AuditLog>
        {
            Items = items,
            TotalCount = totalCount,
            Offset = offset,
            Limit = limit
        };
    }

    public async Task<PagedResult<Tag>> GetTags(
        int offset = 0,
        int limit = 10,
        [Service] IRepository<Tag> repository = null!)
    {
        var query = repository.Query().OrderBy(t => t.TagGroup).ThenBy(t => t.TagOrder);
        var totalCount = await query.CountAsync();
        var items = await query.Skip(offset).Take(limit).ToListAsync();

        return new PagedResult<Tag>
        {
            Items = items,
            TotalCount = totalCount,
            Offset = offset,
            Limit = limit
        };
    }
}
