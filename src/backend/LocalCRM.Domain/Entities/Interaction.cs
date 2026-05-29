using LocalCRM.Domain.Common;
using System;
using System.Collections.Generic;

namespace LocalCRM.Domain.Entities
{
    public class Interaction : BaseEntity
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
        public virtual Interaction? PrevInteraction { get; set; }
        public string? InteractionTags { get; set; }
        public bool IsTask { get; set; }

        public virtual InteractionLink? InteractionLink { get; set; }
        public virtual ICollection<InteractionNoteLink> InteractionNotes { get; set; } = new List<InteractionNoteLink>();
        public virtual ICollection<InteractionDocumentLink> InteractionDocuments { get; set; } = new List<InteractionDocumentLink>();
    }
}
