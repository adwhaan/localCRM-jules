using AutoMapper;
using LocalCRM.Domain.Entities;

namespace LocalCRM.Application.DTOs;

public class CompanyDto
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
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

public class ContactDto
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
    public DateTime CreatedAt { get; set; }
}

public class InteractionDto
{
    public int InteractionId { get; set; }
    public DateOnly InteractionDate { get; set; }
    public TimeOnly? InteractionTime { get; set; }
    public string? Direction { get; set; }
    public string InteractionType { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string? Note { get; set; }
    public string State { get; set; } = string.Empty;
    public bool IsTask { get; set; }
}

public class EngagementDto
{
    public int EngagementId { get; set; }
    public string? EngagementRef { get; set; }
    public string? Description { get; set; }
    public string? EngagementTags { get; set; }
    public string? Confidentiality { get; set; }
    public string EngagementStatus { get; set; } = string.Empty;
}

public class NoteDto
{
    public int NoteId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Body { get; set; }
    public string Visibility { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
}

public class DocumentDto
{
    public int DocumentId { get; set; }
    public string DocumentRef { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string DocumentUrl { get; set; } = string.Empty;
    public string Visibility { get; set; } = string.Empty;
}

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
