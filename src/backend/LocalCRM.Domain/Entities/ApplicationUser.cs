using Microsoft.AspNetCore.Identity;

namespace LocalCRM.Domain.Entities;

public class ApplicationUser : IdentityUser<int>
{
    public bool MustChangePassword { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

public class ApplicationRole : IdentityRole<int>
{
}

public class Permission
{
    public int PermissionId { get; set; }
    public string PermissionName { get; set; } = string.Empty;
}

public class RolePermissionLink
{
    public int RoleId { get; set; }
    public ApplicationRole Role { get; set; } = null!;
    public int PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
}
