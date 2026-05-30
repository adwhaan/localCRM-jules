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

            // 1. Roles
            if (!await roleManager.RoleExistsAsync(Roles.Administrator))
            {
                await roleManager.CreateAsync(new Role { Name = Roles.Administrator });
            }
            if (!await roleManager.RoleExistsAsync(Roles.User))
            {
                await roleManager.CreateAsync(new Role { Name = Roles.User });
            }

            // 2. Permissions
            var permissionNames = typeof(Permissions).GetFields().Select(f => f.GetValue(null)?.ToString()).Where(n => n != null).Cast<string>();
            foreach (var name in permissionNames)
            {
                if (!await context.Permissions.AnyAsync(p => p.PermissionName == name))
                {
                    context.Permissions.Add(new Permission { PermissionName = name });
                }
            }
            await context.SaveChangesAsync();

            // 3. Role-Permission assignments
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
            }

            var userRole = await roleManager.FindByNameAsync(Roles.User);
            if (userRole != null)
            {
                // Assign baseline permissions to User role (example logic)
                var baselinePermissions = new[] { Permissions.CompanyRead, Permissions.ContactRead, Permissions.InteractionRead };
                foreach (var name in baselinePermissions)
                {
                    var p = await context.Permissions.FirstOrDefaultAsync(p => p.PermissionName == name);
                    if (p != null && !await context.RolePermissions.AnyAsync(rp => rp.RoleId == userRole.Id && rp.PermissionId == p.PermissionId))
                    {
                        context.RolePermissions.Add(new RolePermission { RoleId = userRole.Id, PermissionId = p.PermissionId });
                    }
                }
            }
            await context.SaveChangesAsync();

            // 4. Tags (Seed required tag groups if any)
            if (!await context.Tags.AnyAsync())
            {
                context.Tags.Add(new Tag { TagGroup = "interaction_types", TagKey = "call", TagValue = "Call" });
                context.Tags.Add(new Tag { TagGroup = "interaction_types", TagKey = "email", TagValue = "Email" });
                context.Tags.Add(new Tag { TagGroup = "interaction_types", TagKey = "meeting", TagValue = "Meeting" });
            }

            // 5. Settings
            if (!await context.Settings.AnyAsync(s => s.SettingKey == "system_name"))
            {
                context.Settings.Add(new Setting { SettingKey = "system_name", SettingValue = "LocalCRM" });
            }
            await context.SaveChangesAsync();

            // 6. Admin User
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
