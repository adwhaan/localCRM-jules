<<<<<<< HEAD
=======
using HotChocolate.Authorization;
>>>>>>> feature-backend-12855298858282564638
using HotChocolate;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.API.GraphQL.Queries;

<<<<<<< HEAD
=======
[Authorize]
>>>>>>> feature-backend-12855298858282564638
public class RoleQueries
{
    public async Task<List<ApplicationRole>> GetRoles([Service] IRepository<ApplicationRole> repository)
    {
        return await repository.GetAllAsync();
    }
}

<<<<<<< HEAD
=======
[Authorize]
>>>>>>> feature-backend-12855298858282564638
public class PermissionQueries
{
    public async Task<List<Permission>> GetPermissions([Service] IRepository<Permission> repository)
    {
        return await repository.GetAllAsync();
    }
}

<<<<<<< HEAD
=======
[Authorize]
>>>>>>> feature-backend-12855298858282564638
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
