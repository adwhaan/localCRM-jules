using System;

namespace LocalCRM.Application.DTOs
{
    public class InteractionDto
    {
        public int InteractionId { get; set; }
        public DateTime InteractionDate { get; set; }
        public TimeSpan? InteractionTime { get; set; }
        public string? Direction { get; set; }
        public string InteractionType { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string? Note { get; set; }
        public string State { get; set; } = string.Empty;
        public int? PrevInteractionId { get; set; }
        public string? InteractionTags { get; set; }
        public bool IsTask { get; set; }
        public int? ContactId { get; set; }
        public int? CompanyId { get; set; }
        public int? EngagementId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class CreateInteractionDto
    {
        public DateTime InteractionDate { get; set; }
        public TimeSpan? InteractionTime { get; set; }
        public string? Direction { get; set; }
        public string InteractionType { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string? Note { get; set; }
        public string State { get; set; } = string.Empty;
        public int? PrevInteractionId { get; set; }
        public string? InteractionTags { get; set; }
        public bool IsTask { get; set; }
        public int? ContactId { get; set; }
        public int? CompanyId { get; set; }
        public int? EngagementId { get; set; }
    }

    public class UpdateInteractionDto : CreateInteractionDto { }
}
