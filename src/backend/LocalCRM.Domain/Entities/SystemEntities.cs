using System.ComponentModel.DataAnnotations;

namespace LocalCRM.Domain.Entities;

public class Tag
{
    [Key]
    public int TagId { get; set; }
    public string TagGroup { get; set; } = string.Empty;
    public string TagName { get; set; } = string.Empty;
    public string TagValue { get; set; } = string.Empty;
    public int? TagOrder { get; set; }
}

public class Setting
{
    [Key]
    public int SettingId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class AuditLog
{
    [Key]
    public int AuditId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public DateTime PerformedAt { get; set; } = DateTime.UtcNow;
    public string PerformedBy { get; set; } = string.Empty;
    public string? Notes { get; set; }
    
    // New properties
    public string? ClientIp { get; set; } // Adds client IP address for security auditing
}
