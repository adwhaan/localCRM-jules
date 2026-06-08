using LocalCRM.Domain.Common;

namespace LocalCRM.Domain.Entities;

public class Contact : BaseEntity
{
    public int ContactId { get; set; }
    public string? ContactRef { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? LinkedInUrl { get; set; }
    public DateOnly? Birthdate { get; set; }
    public string? ContactTags { get; set; }
    public int Rating { get; set; }
    public string? Sex { get; set; }
}
