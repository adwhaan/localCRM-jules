namespace LocalCRM.Domain.Entities;

public class CompanyContactLink
{
    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    public int ContactId { get; set; }
    public Contact Contact { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Role { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}

public class CompanyNoteLink
{
    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    public int NoteId { get; set; }
    public Note Note { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}

public class CompanyDocumentLink
{
    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    public int DocumentId { get; set; }
    public Document Document { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}

public class ContactNoteLink
{
    public int ContactId { get; set; }
    public Contact Contact { get; set; } = null!;
    public int NoteId { get; set; }
    public Note Note { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}

public class InteractionNoteLink
{
    public int InteractionId { get; set; }
    public Interaction Interaction { get; set; } = null!;
    public int NoteId { get; set; }
    public Note Note { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}

public class InteractionDocumentLink
{
    public int InteractionId { get; set; }
    public Interaction Interaction { get; set; } = null!;
    public int DocumentId { get; set; }
    public Document Document { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}

public class EngagementCompanyLink
{
    public int EngagementId { get; set; }
    public Engagement Engagement { get; set; } = null!;
    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}

public class EngagementContactLink
{
    public int EngagementId { get; set; }
    public Engagement Engagement { get; set; } = null!;
    public int ContactId { get; set; }
    public Contact Contact { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}

public class EngagementNoteLink
{
    public int EngagementId { get; set; }
    public Engagement Engagement { get; set; } = null!;
    public int NoteId { get; set; }
    public Note Note { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}

public class EngagementDocumentLink
{
    public int EngagementId { get; set; }
    public Engagement Engagement { get; set; } = null!;
    public int DocumentId { get; set; }
    public Document Document { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}
