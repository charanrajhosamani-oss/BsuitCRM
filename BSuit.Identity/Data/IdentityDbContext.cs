using BSuit.Contracts.Services;
using BSuit.Identity.Audit;
using BSuit.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Security.AccessControl;

namespace BSuit.Identity.Data
{
    public class IdentityDbContext
        : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private readonly IUserContext _userContext;
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options,
    IUserContext userContext)
            : base(options)
        {
            _userContext = userContext;
        }

        public DbSet<ApplicationAuditLog> ApplicationAuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ✅ Set default schema
            builder.HasDefaultSchema("AUTH");

            // ✅ Optional: Explicit table mapping (recommended for clarity)
            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<ApplicationRole>().ToTable("Roles");

            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
            builder.Entity<ApplicationAuditLog>().ToTable("ApplicationAuditLog");

            //ApplicationUser
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.UserId)
                      .ValueGeneratedNever();

                entity.HasOne(x => x.ReportingManager)
                    .WithMany(x => x.ReportingEmployees)
                    .HasForeignKey(x => x.ReportingManagerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Supervisor)
                    .WithMany(x => x.SupervisedEmployees)
                    .HasForeignKey(x => x.SupervisorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => x.ReportingManagerId);
                entity.HasIndex(x => x.SupervisorId);
            });


            // Remove default unique index on NormalizedName
            builder.Entity<ApplicationRole>()
                .HasIndex(r => r.NormalizedName)
                .IsUnique(false);

            // Create composite unique index
            builder.Entity<ApplicationRole>()
                .HasIndex(r => new { r.TenantId, r.NormalizedName })
                .IsUnique();

        }


        #region SaveChanges Override (Audit Logging)

        public override async Task<int> SaveChangesAsync(
    CancellationToken cancellationToken = default)
        {
            var auditEntries = new List<(EntityEntry Entry, ApplicationAuditLog Audit)>();

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is ApplicationAuditLog ||
                    entry.State == EntityState.Detached ||
                    entry.State == EntityState.Unchanged)
                {
                    continue;
                }

                var audit = AuditHelper.CreateAuditLog(
                    entry,
                    _userContext?.UserId,
                    _userContext?.TenantId);

                if (audit != null)
                {
                    auditEntries.Add((entry, audit));
                }
            }

            var result = await base.SaveChangesAsync(cancellationToken);

            // Update key values after insert
            foreach (var item in auditEntries)
            {
                if (item.Entry.State == EntityState.Added)
                {
                    var keys = item.Entry.Properties
                        .Where(p => p.Metadata.IsPrimaryKey())
                        .ToDictionary(
                            p => p.Metadata.Name,
                            p => p.CurrentValue
                        );

                    item.Audit.KeyValues =
                        System.Text.Json.JsonSerializer.Serialize(keys);
                }
            }

            if (auditEntries.Any())
            {
                ApplicationAuditLogs.AddRange(
                    auditEntries.Select(x => x.Audit));

                await base.SaveChangesAsync(cancellationToken);
            }

            return result;
        }
        #endregion

    }
}