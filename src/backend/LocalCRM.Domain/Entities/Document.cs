using LocalCRM.Domain.Common;
using System.Collections.Generic;

namespace LocalCRM.Domain.Entities
{
    public class Document : BaseEntity
    {
        public int DocumentId { get; set; }
        public string DocumentRef { get; set; } = string.Empty;
        public string? Subject { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentUrl { get; set; } = string.Empty;
        public string Visibility { get; set; } = string.Empty;
        public string? Checksum { get; set; }
        public bool IsChecked { get; set; }

        public virtual ICollection<CompanyDocumentLink> CompanyDocuments { get; set; } = new List<CompanyDocumentLink>();
        public virtual ICollection<InteractionDocumentLink> InteractionDocuments { get; set; } = new List<InteractionDocumentLink>();
        public virtual ICollection<EngagementDocumentLink> EngagementDocuments { get; set; } = new List<EngagementDocumentLink>();
    }
}
