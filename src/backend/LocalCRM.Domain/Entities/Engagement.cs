using LocalCRM.Domain.Common;
using System.Collections.Generic;

namespace LocalCRM.Domain.Entities
{
    public class Engagement : BaseEntity
    {
        public int EngagementId { get; set; }
        public string? EngagementRef { get; set; }
        public string? Description { get; set; }
        public string? EngagementTags { get; set; }
        public string? Confidentiality { get; set; }
        public string EngagementStatus { get; set; } = string.Empty;

        public virtual ICollection<EngagementCompanyLink> EngagementCompanies { get; set; } = new List<EngagementCompanyLink>();
        public virtual ICollection<EngagementContactLink> EngagementContacts { get; set; } = new List<EngagementContactLink>();
        public virtual ICollection<EngagementNoteLink> EngagementNotes { get; set; } = new List<EngagementNoteLink>();
        public virtual ICollection<EngagementDocumentLink> EngagementDocuments { get; set; } = new List<EngagementDocumentLink>();
        public virtual ICollection<InteractionLink> InteractionLinks { get; set; } = new List<InteractionLink>();
    }
}
