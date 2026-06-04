using LocalCRM.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Interaction> Interactions => Set<Interaction>();
    public DbSet<InteractionLink> InteractionLinks => Set<InteractionLink>();
    public DbSet<Engagement> Engagements => Set<Engagement>();
    public DbSet<Note> Notes => Set<Note>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Setting> Settings => Set<Setting>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<CompanyContactLink> CompanyContactLinks => Set<CompanyContactLink>();
    public DbSet<CompanyNoteLink> CompanyNoteLinks => Set<CompanyNoteLink>();
    public DbSet<CompanyDocumentLink> CompanyDocumentLinks => Set<CompanyDocumentLink>();
    public DbSet<ContactNoteLink> ContactNoteLinks => Set<ContactNoteLink>();
    public DbSet<InteractionNoteLink> InteractionNoteLinks => Set<InteractionNoteLink>();
    public DbSet<InteractionDocumentLink> InteractionDocumentLinks => Set<InteractionDocumentLink>();
    public DbSet<EngagementCompanyLink> EngagementCompanyLinks => Set<EngagementCompanyLink>();
    public DbSet<EngagementContactLink> EngagementContactLinks => Set<EngagementContactLink>();
    public DbSet<EngagementNoteLink> EngagementNoteLinks => Set<EngagementNoteLink>();
    public DbSet<EngagementDocumentLink> EngagementDocumentLinks => Set<EngagementDocumentLink>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure Composite Keys
        builder.Entity<CompanyContactLink>()
            .HasKey(c => new { c.CompanyId, c.ContactId, c.StartDate });

        builder.Entity<InteractionLink>()
            .HasKey(i => i.InteractionId);

        builder.Entity<CompanyNoteLink>()
            .HasKey(c => new { c.CompanyId, c.NoteId });

        builder.Entity<CompanyDocumentLink>()
            .HasKey(c => new { c.CompanyId, c.DocumentId });

        builder.Entity<ContactNoteLink>()
            .HasKey(c => new { c.ContactId, c.NoteId });

        builder.Entity<InteractionNoteLink>()
            .HasKey(i => new { i.InteractionId, i.NoteId });

        builder.Entity<InteractionDocumentLink>()
            .HasKey(i => new { i.InteractionId, i.DocumentId });

        builder.Entity<EngagementCompanyLink>()
            .HasKey(e => new { e.EngagementId, e.CompanyId, e.StartDate });

        builder.Entity<EngagementContactLink>()
            .HasKey(e => new { e.EngagementId, e.ContactId, e.StartDate });

        builder.Entity<EngagementNoteLink>()
            .HasKey(e => new { e.EngagementId, e.NoteId });

        builder.Entity<EngagementDocumentLink>()
            .HasKey(e => new { e.EngagementId, e.DocumentId });

        // Naming conventions and defaults
        builder.Entity<Company>().ToTable("companies");
        builder.Entity<Contact>().ToTable("contacts");
        builder.Entity<Interaction>().ToTable("interactions");
        builder.Entity<InteractionLink>().ToTable("interactions_link");
        builder.Entity<Engagement>().ToTable("engagements");
        builder.Entity<Note>().ToTable("notes");
        builder.Entity<Document>().ToTable("documents");
        builder.Entity<Tag>().ToTable("tags");
        builder.Entity<Setting>().ToTable("settings");
        builder.Entity<AuditLog>().ToTable("audit_logs");
        builder.Entity<RefreshToken>().ToTable("refresh_tokens");

        builder.Entity<CompanyContactLink>().ToTable("company_contacts_link");
        builder.Entity<CompanyNoteLink>().ToTable("company_notes_link");
        builder.Entity<CompanyDocumentLink>().ToTable("company_documents_link");
        builder.Entity<ContactNoteLink>().ToTable("contact_notes_link");
        builder.Entity<InteractionNoteLink>().ToTable("interactions_notes_link");
        builder.Entity<InteractionDocumentLink>().ToTable("interactions_documents_link");
        builder.Entity<EngagementCompanyLink>().ToTable("engagement_companies_link");
        builder.Entity<EngagementContactLink>().ToTable("engagement_contacts_link");
        builder.Entity<EngagementNoteLink>().ToTable("engagement_notes_link");
        builder.Entity<EngagementDocumentLink>().ToTable("engagement_documents_link");

        // Exclusivity rule for InteractionLink
        // This is harder to enforce at DB level in SQLite without triggers, but we can document intent
    }
}
