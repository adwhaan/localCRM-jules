using System;

namespace LocalCRM.Domain.Entities
{
    // Common properties for soft-deletable links
    public abstract class BaseLink
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }

    public class CompanyContactLink : BaseLink
    {
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; } = null!;
        public int ContactId { get; set; }
        public virtual Contact Contact { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Role { get; set; }
    }

    public class InteractionLink : BaseLink
    {
        public int InteractionId { get; set; }
        public virtual Interaction Interaction { get; set; } = null!;
        public int? ContactId { get; set; }
        public virtual Contact? Contact { get; set; }
        public int? CompanyId { get; set; }
        public virtual Company? Company { get; set; }
        public int? EngagementId { get; set; }
        public virtual Engagement? Engagement { get; set; }
    }

    public class CompanyNoteLink : BaseLink
    {
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; } = null!;
        public int NoteId { get; set; }
        public virtual Note Note { get; set; } = null!;
    }

    public class CompanyDocumentLink : BaseLink
    {
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; } = null!;
        public int DocumentId { get; set; }
        public virtual Document Document { get; set; } = null!;
    }

    public class ContactNoteLink : BaseLink
    {
        public int ContactId { get; set; }
        public virtual Contact Contact { get; set; } = null!;
        public int NoteId { get; set; }
        public virtual Note Note { get; set; } = null!;
    }

    public class InteractionNoteLink : BaseLink
    {
        public int InteractionId { get; set; }
        public virtual Interaction Interaction { get; set; } = null!;
        public int NoteId { get; set; }
        public virtual Note Note { get; set; } = null!;
    }

    public class InteractionDocumentLink : BaseLink
    {
        public int InteractionId { get; set; }
        public virtual Interaction Interaction { get; set; } = null!;
        public int DocumentId { get; set; }
        public virtual Document Document { get; set; } = null!;
    }

    public class EngagementCompanyLink : BaseLink
    {
        public int EngagementId { get; set; }
        public virtual Engagement Engagement { get; set; } = null!;
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class EngagementContactLink : BaseLink
    {
        public int EngagementId { get; set; }
        public virtual Engagement Engagement { get; set; } = null!;
        public int ContactId { get; set; }
        public virtual Contact Contact { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class EngagementNoteLink : BaseLink
    {
        public int EngagementId { get; set; }
        public virtual Engagement Engagement { get; set; } = null!;
        public int NoteId { get; set; }
        public virtual Note Note { get; set; } = null!;
    }

    public class EngagementDocumentLink : BaseLink
    {
        public int EngagementId { get; set; }
        public virtual Engagement Engagement { get; set; } = null!;
        public int DocumentId { get; set; }
        public virtual Document Document { get; set; } = null!;
    }
}
