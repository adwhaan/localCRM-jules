using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocalCRM.Infrastructure.Persistence.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("companies");
            builder.HasKey(e => e.CompanyId);
            builder.Property(e => e.CompanyId).HasColumnName("company_id");
            builder.Property(e => e.CompanyRef).IsRequired().HasColumnName("company_ref");
            builder.HasIndex(e => e.CompanyRef).IsUnique();
            builder.Property(e => e.Name).IsRequired().HasColumnName("name");
            builder.Property(e => e.UpdatedAt).IsConcurrencyToken();

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }

    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.ToTable("contacts");
            builder.HasKey(e => e.ContactId);
            builder.Property(e => e.ContactId).HasColumnName("contact_id");
            builder.Property(e => e.FirstName).IsRequired().HasColumnName("first_name");
            builder.Property(e => e.LastName).IsRequired().HasColumnName("last_name");
            builder.Property(e => e.UpdatedAt).IsConcurrencyToken();

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }

    public class InteractionConfiguration : IEntityTypeConfiguration<Interaction>
    {
        public void Configure(EntityTypeBuilder<Interaction> builder)
        {
            builder.ToTable("interactions");
            builder.HasKey(e => e.InteractionId);
            builder.Property(e => e.InteractionId).HasColumnName("interaction_id");
            builder.Property(e => e.Subject).IsRequired().HasColumnName("subject");
            builder.Property(e => e.UpdatedAt).IsConcurrencyToken();

            builder.HasOne(e => e.PrevInteraction)
                .WithMany()
                .HasForeignKey(e => e.PrevInteractionId);

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }

    public class EngagementConfiguration : IEntityTypeConfiguration<Engagement>
    {
        public void Configure(EntityTypeBuilder<Engagement> builder)
        {
            builder.ToTable("engagements");
            builder.HasKey(e => e.EngagementId);
            builder.Property(e => e.EngagementId).HasColumnName("engagement_id");
            builder.Property(e => e.UpdatedAt).IsConcurrencyToken();

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }

    public class NoteConfiguration : IEntityTypeConfiguration<Note>
    {
        public void Configure(EntityTypeBuilder<Note> builder)
        {
            builder.ToTable("notes");
            builder.HasKey(e => e.NoteId);
            builder.Property(e => e.NoteId).HasColumnName("note_id");
            builder.Property(e => e.UpdatedAt).IsConcurrencyToken();

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }

    public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.ToTable("documents");
            builder.HasKey(e => e.DocumentId);
            builder.Property(e => e.DocumentId).HasColumnName("document_id");
            builder.Property(e => e.UpdatedAt).IsConcurrencyToken();

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }

    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable("tags");
            builder.HasKey(e => e.TagId);
            builder.Property(e => e.TagId).HasColumnName("tag_id");
        }
    }

    public class SettingConfiguration : IEntityTypeConfiguration<Setting>
    {
        public void Configure(EntityTypeBuilder<Setting> builder)
        {
            builder.ToTable("settings");
            builder.HasKey(e => e.SettingId);
            builder.Property(e => e.SettingId).HasColumnName("setting_id");
            builder.HasIndex(e => e.Key).IsUnique();
        }
    }

    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("audit_logs");
            builder.HasKey(e => e.AuditId);
            builder.Property(e => e.AuditId).HasColumnName("audit_id");
        }
    }

    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("refresh_tokens");
            builder.HasKey(e => e.RefreshTokenId);
            builder.Property(e => e.RefreshTokenId).HasColumnName("refresh_token_id");

            builder.HasOne(e => e.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(e => e.UserId);

            builder.HasOne(e => e.ReplacedByToken)
                .WithMany()
                .HasForeignKey(e => e.ReplacedByTokenId);
        }
    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");
            builder.Property(e => e.Id).HasColumnName("user_id");
            builder.Property(e => e.UpdatedAt).IsConcurrencyToken();
        }
    }

    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("roles");
            builder.Property(e => e.Id).HasColumnName("role_id");
        }
    }

    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("permissions");
            builder.HasKey(e => e.PermissionId);
            builder.Property(e => e.PermissionId).HasColumnName("permission_id");
        }
    }
}
