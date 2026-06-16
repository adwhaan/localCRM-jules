using LocalCRM.Domain.Common;

namespace LocalCRM.Domain.Entities;

public class Company : BaseEntity
{
    public int CompanyId { get; set; }
    public string CompanyRef { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? PostalCode { get; set; }
    public string City { get; set; } = string.Empty;
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? Email { get; set; }
    public string? CompanyTags { get; set; }
    public string CompanyType { get; set; } = string.Empty;
    public string? Branch { get; set; }
    public string? Size { get; set; }
    public int Rating { get; set; }
}
