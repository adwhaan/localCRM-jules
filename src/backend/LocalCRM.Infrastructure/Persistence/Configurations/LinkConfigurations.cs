using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocalCRM.Infrastructure.Persistence.Configurations
{
    public class CompanyContactLinkConfiguration : IEntityTypeConfiguration<CompanyContactLink>
    {
        public void Configure(EntityTypeBuilder<CompanyContactLink> builder)
        {
            builder.ToTable("company_contacts_link");
            builder.HasKey(e => new { e.CompanyId, e.ContactId, e.StartDate });

            builder.HasOne(e => e.Company)
                .WithMany(c => c.CompanyContacts)
                .HasForeignKey(e => e.CompanyId);

            builder.HasOne(e => e.Contact)
                .WithMany(c => c.CompanyContacts)
                .HasForeignKey(e => e.ContactId);

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }

    public class InteractionLinkConfiguration : IEntityTypeConfiguration<InteractionLink>
    {
        public void Configure(EntityTypeBuilder<InteractionLink> builder)
        {
            builder.ToTable("interactions_link");
            builder.HasKey(e => e.InteractionId);

            builder.HasOne(e => e.Interaction)
                .WithOne(i => i.InteractionLink)
                .HasForeignKey<InteractionLink>(e => e.InteractionId);

            builder.HasOne(e => e.Contact)
                .WithMany(c => c.InteractionLinks)
                .HasForeignKey(e => e.ContactId);

            builder.HasOne(e => e.Company)
                .WithMany(c => c.InteractionLinks)
                .HasForeignKey(e => e.CompanyId);

            builder.HasOne(e => e.Engagement)
                .WithMany(e => e.InteractionLinks)
                .HasForeignKey(e => e.EngagementId);

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }

    public class CompanyNoteLinkConfiguration : IEntityTypeConfiguration<CompanyNoteLink>
    {
        public void Configure(EntityTypeBuilder<CompanyNoteLink> builder)
        {
            builder.ToTable("company_notes_link");
            builder.HasKey(e => new { e.CompanyId, e.NoteId });

            builder.HasOne(e => e.Company)
                .WithMany(c => c.CompanyNotes)
                .HasForeignKey(e => e.CompanyId);

            builder.HasOne(e => e.Note)
                .WithMany(n => n.CompanyNotes)
                .HasForeignKey(e => e.NoteId);

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }

    public class CompanyDocumentLinkConfiguration : IEntityTypeConfiguration<CompanyDocumentLink>
    {
        public void Configure(EntityTypeBuilder<CompanyDocumentLink> builder)
        {
            builder.ToTable("company_documents_link");
            builder.HasKey(e => new { e.CompanyId, e.DocumentId });

            builder.HasOne(e => e.Company)
                .WithMany(c => c.CompanyDocuments)
                .HasForeignKey(e => e.CompanyId);

            builder.HasOne(e => e.Document)
                .WithMany(d => d.CompanyDocuments)
                .HasForeignKey(e => e.DocumentId);

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }

    public class ContactNoteLinkConfiguration : IEntityTypeConfiguration<ContactNoteLink>
    {
        public void Configure(EntityTypeBuilder<ContactNoteLink> builder)
        {
            builder.ToTable("contact_notes_link");
            builder.HasKey(e => new { e.ContactId, e.NoteId });

            builder.HasOne(e => e.Contact)
                .WithMany(c => c.ContactNotes)
                .HasForeignKey(e => e.ContactId);

            builder.HasOne(e => e.Note)
                .WithMany(n => n.ContactNotes)
                .HasForeignKey(e => e.NoteId);

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }

    public class InteractionNoteLinkConfiguration : IEntityTypeConfiguration<InteractionNoteLink>
    {
        public void Configure(EntityTypeBuilder<InteractionNoteLink> builder)
        {
            builder.ToTable("interactions_notes_link");
            builder.HasKey(e => new { e.InteractionId, e.NoteId });

            builder.HasOne(e => e.Interaction)
                .WithMany(i => i.InteractionNotes)
                .HasForeignKey(e => e.InteractionId);

            builder.HasOne(e => e.Note)
                .WithMany(n => n.InteractionNotes)
                .HasForeignKey(e => e.NoteId);

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }

    public class InteractionDocumentLinkConfiguration : IEntityTypeConfiguration<InteractionDocumentLink>
    {
        public void Configure(EntityTypeBuilder<InteractionDocumentLink> builder)
        {
            builder.ToTable("interactions_documents_link");
            builder.HasKey(e => new { e.InteractionId, e.DocumentId });

            builder.HasOne(e => e.Interaction)
                .WithMany(i => i.InteractionDocuments)
                .HasForeignKey(e => e.InteractionId);

            builder.HasOne(e => e.Document)
                .WithMany(d => d.InteractionDocuments)
                .HasForeignKey(e => e.DocumentId);

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }

    public class EngagementCompanyLinkConfiguration : IEntityTypeConfiguration<EngagementCompanyLink>
    {
        public void Configure(EntityTypeBuilder<EngagementCompanyLink> builder)
        {
            builder.ToTable("engagement_companies_link");
            builder.HasKey(e => new { e.EngagementId, e.CompanyId, e.StartDate });

            builder.HasOne(e => e.Engagement)
                .WithMany(e => e.EngagementCompanies)
                .HasForeignKey(e => e.EngagementId);

            builder.HasOne(e => e.Company)
                .WithMany(c => c.EngagementCompanies)
                .HasForeignKey(e => e.CompanyId);

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }

    public class EngagementContactLinkConfiguration : IEntityTypeConfiguration<EngagementContactLink>
    {
        public void Configure(EntityTypeBuilder<EngagementContactLink> builder)
        {
            builder.ToTable("engagement_contacts_link");
            builder.HasKey(e => new { e.EngagementId, e.ContactId, e.StartDate });

            builder.HasOne(e => e.Engagement)
                .WithMany(e => e.EngagementContacts)
                .HasForeignKey(e => e.EngagementId);

            builder.HasOne(e => e.Contact)
                .WithMany(c => c.EngagementContacts)
                .HasForeignKey(e => e.ContactId);

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }

    public class EngagementNoteLinkConfiguration : IEntityTypeConfiguration<EngagementNoteLink>
    {
        public void Configure(EntityTypeBuilder<EngagementNoteLink> builder)
        {
            builder.ToTable("engagement_notes_link");
            builder.HasKey(e => new { e.EngagementId, e.NoteId });

            builder.HasOne(e => e.Engagement)
                .WithMany(e => e.EngagementNotes)
                .HasForeignKey(e => e.EngagementId);

            builder.HasOne(e => e.Note)
                .WithMany(n => n.EngagementNotes)
                .HasForeignKey(e => e.NoteId);

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }

    public class EngagementDocumentLinkConfiguration : IEntityTypeConfiguration<EngagementDocumentLink>
    {
        public void Configure(EntityTypeBuilder<EngagementDocumentLink> builder)
        {
            builder.ToTable("engagement_documents_link");
            builder.HasKey(e => new { e.EngagementId, e.DocumentId });

            builder.HasOne(e => e.Engagement)
                .WithMany(e => e.EngagementDocuments)
                .HasForeignKey(e => e.EngagementId);

            builder.HasOne(e => e.Document)
                .WithMany(d => d.EngagementDocuments)
                .HasForeignKey(e => e.DocumentId);

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
