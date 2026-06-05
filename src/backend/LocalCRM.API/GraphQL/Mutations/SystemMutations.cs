using HotChocolate.Authorization;
using HotChocolate;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using LocalCRM.API.GraphQL.Common;

namespace LocalCRM.API.GraphQL.Mutations;

[Authorize]
public class SettingMutations
{
    public async Task<Setting> UpdateSetting(string key, string value, [Service] IRepository<Setting> repository)
    {
        var setting = await repository.Query().FirstOrDefaultAsync(s => s.Key == key);
        if (setting == null)
        {
            setting = new Setting { Key = key, Value = value };
            await repository.AddAsync(setting);
        }
        else
        {
            setting.Value = value;
            await repository.UpdateAsync(setting);
        }
        return setting;
    }
}

<<<<<<< HEAD
=======
[Authorize]
>>>>>>> feature-backend-12855298858282564638
public class RoleMutations
{
    public async Task<MutationResult> AssignPermissionToRole(int roleId, int permissionId, [Service] IRepository<RolePermissionLink> repository)
    {
        var link = new RolePermissionLink { RoleId = roleId, PermissionId = permissionId };
        await repository.AddAsync(link);
        return new MutationResult(true);
    }

    public async Task<MutationResult> RemovePermissionFromRole(int roleId, int permissionId, [Service] IRepository<RolePermissionLink> repository)
    {
        var link = await repository.Query().FirstOrDefaultAsync(l => l.RoleId == roleId && l.PermissionId == permissionId);
        if (link != null) await repository.DeleteAsync(link);
        return new MutationResult(true);
    }
}
