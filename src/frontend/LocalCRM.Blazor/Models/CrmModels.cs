using System;
using System.Collections.Generic;

namespace LocalCRM.Blazor.Models;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
}

public class CompanyDto
{
    public int CompanyId { get; set; }
    public string CompanyRef { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? CompanyType { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ContactDto
{
    public int ContactId { get; set; }
    public string? ContactRef { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class InteractionDto
{
    public int InteractionId { get; set; }
    public DateTime InteractionDate { get; set; }
    public string InteractionType { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public bool IsTask { get; set; }
}

public class EngagementDto
{
    public int EngagementId { get; set; }
    public string? EngagementRef { get; set; }
    public string? Description { get; set; }
    public string EngagementStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class UserDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
}

public class DashboardMetrics
{
    public int TotalCompanies { get; set; }
    public int TotalContacts { get; set; }
    public int TotalEngagements { get; set; }
    public int TotalInteractions { get; set; }
    public int UpcomingTasks { get; set; }
}
