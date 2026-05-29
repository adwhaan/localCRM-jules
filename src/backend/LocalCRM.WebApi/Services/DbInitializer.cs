using LocalCRM.Domain.Entities;
using LocalCRM.Domain.Enums;
using LocalCRM.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocalCRM.WebApi.Services
{
    public class DbInitializer
    {
        public static async Task Initialize(LocalCRMContext context, UserManager<User> userManager, RoleManager<Role> roleManager, string adminPassword)
        {
            await context.Database.MigrateAsync();

            if (!await roleManager.RoleExistsAsync(Roles.Administrator))
            {
                await roleManager.CreateAsync(new Role { Name = Roles.Administrator });
            }
            if (!await roleManager.RoleExistsAsync(Roles.User))
            {
                await roleManager.CreateAsync(new Role { Name = Roles.User });
            }

            var permissionNames = typeof(Permissions).GetFields().Select(f => f.GetValue(null)?.ToString()).Where(n => n != null).Cast<string>();
            foreach (var name in permissionNames)
            {
                if (!await context.Permissions.AnyAsync(p => p.PermissionName == name))
                {
                    context.Permissions.Add(new Permission { PermissionName = name });
                }
            }
            await context.SaveChangesAsync();

            var adminRole = await roleManager.FindByNameAsync(Roles.Administrator);
            if (adminRole != null)
            {
                var allPermissions = await context.Permissions.ToListAsync();
                foreach (var p in allPermissions)
                {
                    if (!await context.RolePermissions.AnyAsync(rp => rp.RoleId == adminRole.Id && rp.PermissionId == p.PermissionId))
                    {
                        context.RolePermissions.Add(new RolePermission { RoleId = adminRole.Id, PermissionId = p.PermissionId });
                    }
                }
                await context.SaveChangesAsync();
            }

            if (await userManager.FindByNameAsync("admin") == null)
            {
                var user = new User
                {
                    UserName = "admin",
                    Email = "admin@localcrm.local",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system"
                };
                var result = await userManager.CreateAsync(user, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, Roles.Administrator);
                }
            }
        }
    }
}
