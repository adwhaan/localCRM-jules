using LocalCRM.Domain.Common;

namespace LocalCRM.Domain.Entities;

public class Interaction : BaseEntity
{
    public int InteractionId { get; set; }
    public DateOnly InteractionDate { get; set; }
    public TimeOnly? InteractionTime { get; set; }
    public string? Direction { get; set; }
    public string InteractionType { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string? Note { get; set; }
    public string State { get; set; } = string.Empty;
    public int? PrevInteractionId { get; set; }
    public Interaction? PrevInteraction { get; set; }
    public string? InteractionTags { get; set; }
    public bool IsTask { get; set; }
    public InteractionLink? InteractionLink { get; set; }
}
