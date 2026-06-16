namespace LocalCRM.Blazor.Models;

public class CompanyDto
{
    public int CompanyId { get; set; }
    public string CompanyRef { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? Country { get; set; }
    public string CompanyType { get; set; } = string.Empty;
}

public class ContactDto
{
    public int ContactId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
}

public class EngagementDto
{
    public int EngagementId { get; set; }
    public string? EngagementRef { get; set; }
    public string? Description { get; set; }
    public string EngagementStatus { get; set; } = string.Empty;
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
}
