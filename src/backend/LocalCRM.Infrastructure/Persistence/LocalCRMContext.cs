using LocalCRM.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Infrastructure.Persistence
{
    public class LocalCRMContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public LocalCRMContext(DbContextOptions<LocalCRMContext> options) : base(options) { }

        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<Company> Companies => Set<Company>();
        public DbSet<Contact> Contacts => Set<Contact>();
        public DbSet<Interaction> Interactions => Set<Interaction>();
        public DbSet<Engagement> Engagements => Set<Engagement>();
        public DbSet<Note> Notes => Set<Note>();
        public DbSet<Document> Documents => Set<Document>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<Setting> Settings => Set<Setting>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        public DbSet<CompanyContactLink> CompanyContactLinks => Set<CompanyContactLink>();
        public DbSet<InteractionLink> InteractionLinks => Set<InteractionLink>();
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

            builder.ApplyConfigurationsFromAssembly(typeof(LocalCRMContext).Assembly);

            // Identity relationships
            builder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            builder.Entity<RolePermission>(rolePermission =>
            {
                rolePermission.HasKey(rp => new { rp.RoleId, rp.PermissionId });

                rolePermission.HasOne(rp => rp.Role)
                    .WithMany(r => r.RolePermissions)
                    .HasForeignKey(rp => rp.RoleId);

                rolePermission.HasOne(rp => rp.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(rp => rp.PermissionId);
            });
        }
    }
}
