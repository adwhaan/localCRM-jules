using LocalCRM.Domain.Common;
using System.Collections.Generic;

namespace LocalCRM.Domain.Entities
{
    public class Note : BaseEntity
    {
        public int NoteId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string? Body { get; set; }
        public string Visibility { get; set; } = string.Empty;

        public virtual ICollection<CompanyNoteLink> CompanyNotes { get; set; } = new List<CompanyNoteLink>();
        public virtual ICollection<ContactNoteLink> ContactNotes { get; set; } = new List<ContactNoteLink>();
        public virtual ICollection<InteractionNoteLink> InteractionNotes { get; set; } = new List<InteractionNoteLink>();
        public virtual ICollection<EngagementNoteLink> EngagementNotes { get; set; } = new List<EngagementNoteLink>();
    }
}
