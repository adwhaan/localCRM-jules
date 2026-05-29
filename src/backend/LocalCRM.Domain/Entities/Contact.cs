using LocalCRM.Domain.Common;
using System;
using System.Collections.Generic;

namespace LocalCRM.Domain.Entities
{
    public class Contact : BaseEntity
    {
        public int ContactId { get; set; }
        public string? ContactRef { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? LinkedinUrl { get; set; }
        public DateTime? Birthdate { get; set; }
        public string? ContactTags { get; set; }
        public int Rating { get; set; }
        public string? Sex { get; set; }

        public virtual ICollection<CompanyContactLink> CompanyContacts { get; set; } = new List<CompanyContactLink>();
        public virtual ICollection<ContactNoteLink> ContactNotes { get; set; } = new List<ContactNoteLink>();
        public virtual ICollection<EngagementContactLink> EngagementContacts { get; set; } = new List<EngagementContactLink>();
        public virtual ICollection<InteractionLink> InteractionLinks { get; set; } = new List<InteractionLink>();
    }
}
