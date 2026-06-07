namespace LocalCRM.Blazor.Models;

public class TagDto
{
    public int TagId { get; set; }
    public string TagGroup { get; set; } = string.Empty;
    public string TagName { get; set; } = string.Empty;
    public string TagValue { get; set; } = string.Empty;
}

public class SettingDto
{
    public int SettingId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class AuditLogDto
{
    public int AuditId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public DateTime PerformedAt { get; set; }
    public string PerformedBy { get; set; } = string.Empty;
}
