namespace LocalCRM.Domain.Entities;

public class InteractionLink
{
    public int InteractionId { get; set; }
    public Interaction Interaction { get; set; } = null!;
    public int? ContactId { get; set; }
    public Contact? Contact { get; set; }
    public int? CompanyId { get; set; }
    public Company? Company { get; set; }
    public int? EngagementId { get; set; }
    public Engagement? Engagement { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}