using BSuit.Contracts.Services;
using BSuit.Core.Audit;
using BSuit.Core.Entities;
using BSuit.Core.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BSuit.Core.Data
{
    public class CoreDbContext : DbContext
    {
        private readonly IUserContext? _userContext;
        private bool _piiLoaded = false;
        public CoreDbContext(
             DbContextOptions<CoreDbContext> options,
             IUserContext? userContext = null)
             : base(options)
        {
            _userContext = userContext;

            if (_userContext != null)
            {
                //ChangeTracker.Tracked += OnEntityTracked;
                //LoadPIIConfigCache();
            }
        }

       

        #region DbSets
        public DbSet<TenantMaster> Tenants { get; set; }
        public DbSet<ModuleMaster> Modules { get; set; }
        public DbSet<SubscriptionMaster> Subscriptions { get; set; }
        public DbSet<TenantSubscription> TenantSubscriptions { get; set; }
        public DbSet<TenantSubscriptionModule> TenantSubscriptionModules { get; set; }


        public DbSet<EncryptionKey> EncryptionKeys { get; set; }
        public DbSet<PIIConfig> PIIConfigs { get; set; }

        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<LogEntry> Logs { get; set; }


        //Admin Access
        public DbSet<Menu> Menus { get; set; }
        public DbSet<RoleMenu> RoleMenus { get; set; }



        public DbSet<PermissionMaster> PermissionMasters { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<ColumnPermission> ColumnPermissions { get; set; }



        public DbSet<NotificationMaster> NotificationMasters { get; set; }
        public DbSet<NotificationRecipient> NotificationRecipients { get; set; }

        public DbSet<BusinessMenuMaster> BusinessMenuMasters { get; set; }
        public DbSet<BusinessRoleMenuMapping> BusinessRoleMenuMappings { get; set; }
        #endregion



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ✅ Set default schema
            builder.HasDefaultSchema("CORE");

            // ✅ Table naming (optional but clean)
            builder.Entity<TenantMaster>().ToTable("TenantMaster");
            builder.Entity<ModuleMaster>().ToTable("ModuleMaster");
            builder.Entity<SubscriptionMaster>().ToTable("SubscriptionMaster");
            builder.Entity<TenantSubscription>().ToTable("TenantSubscription");
            builder.Entity<TenantSubscriptionModule>().ToTable("TenantSubscriptionModule");
            builder.Entity<AuditLog>().ToTable("AuditLog");

            // ✅ Relationships

            builder.Entity<TenantSubscription>()
                .HasOne(ts => ts.Tenant)
                .WithMany(t => t.Subscriptions)
                .HasForeignKey(ts => ts.TenantId);

            builder.Entity<TenantSubscription>()
                .HasOne(ts => ts.Subscription)
                .WithMany(s => s.TenantSubscriptions)
                .HasForeignKey(ts => ts.SubscriptionId);

            builder.Entity<TenantSubscriptionModule>()
                .HasOne(tsm => tsm.TenantSubscription)
                .WithMany(ts => ts.Modules)
                .HasForeignKey(tsm => tsm.TenantSubscriptionId);

            builder.Entity<TenantSubscriptionModule>()
                .HasOne(tsm => tsm.Module)
                .WithMany()
                .HasForeignKey(tsm => tsm.ModuleId);

            // ✅ Indexes (important for performance)
            builder.Entity<TenantMaster>()
                .HasIndex(t => t.Name)
                .IsUnique();

            builder.Entity<ModuleMaster>()
                .HasIndex(m => m.Code)
                .IsUnique();

            builder.Entity<TenantMaster>().HasQueryFilter(x => x.IsActive);

            builder.Entity<LogEntry>(entity =>
            {
                entity.ToTable("Logs");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Level).HasMaxLength(50);
                entity.Property(x => x.SourceContext).HasMaxLength(500);
                entity.Property(x => x.RequestPath).HasMaxLength(500);

                entity.Property(x => x.Message).HasColumnType("nvarchar(max)");
                entity.Property(x => x.Exception).HasColumnType("nvarchar(max)");
                entity.Property(x => x.Properties).HasColumnType("nvarchar(max)");
                entity.Property(x => x.LogEvent).HasColumnType("nvarchar(max)");
                entity.Property(x => x.Remarks).HasColumnType("nvarchar(500)");


                entity.HasIndex(x => x.Timestamp);
                entity.HasIndex(x => x.Level);
            });



            builder.Entity<NotificationRecipient>()
                 .HasOne(nr => nr.Notification)
                 .WithMany(n => n.Recipients)
                 .HasForeignKey(nr => nr.NotificationMasterId)
                 .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<BusinessMenuMaster>()
                .HasOne(x => x.ParentMenu)
                .WithMany(x => x.ChildMenus)
                .HasForeignKey(x => x.ParentMenuId)
                .OnDelete(DeleteBehavior.Restrict);
        }




        #region SaveChanges Override (PII Encryption and Audit Logging)

        public override async Task<int> SaveChangesAsync(
                CancellationToken cancellationToken = default)
        {
            var auditEntries = new List<AuditLog>();

            //----------------------------------------
            // STEP 1: Try PII Encryption
            //----------------------------------------
            try
            {
                var piiConfigs = await PIIConfigs
                    .Where(x => x.IsActive)
                    .ToListAsync(cancellationToken);

                var changedEntries = ChangeTracker.Entries()
                    .Where(x =>
                        x.State == EntityState.Added ||
                        x.State == EntityState.Modified)
                    .ToList();

                foreach (var entry in changedEntries)
                {
                    if (entry.Entity is AuditLog)
                        continue;

                    var tableName = entry.Metadata.GetTableName();

                    var configs = piiConfigs
                        .Where(x =>
                            x.TableName == tableName &&
                            (
                                x.TenantId == null ||
                                x.TenantId == _userContext?.TenantId
                            ))
                        .ToList();

                    foreach (var config in configs)
                    {
                        try
                        {
                            var property = entry.Properties
                                .FirstOrDefault(p =>
                                    p.Metadata.Name == config.ColumnName);

                            if (property?.CurrentValue == null)
                                continue;

                            var value = property.CurrentValue.ToString();

                            if (string.IsNullOrWhiteSpace(value))
                                continue;

                            if (EncryptionHelper.IsEncrypted(value))
                                continue;

                            property.CurrentValue =
                                EncryptionHelper.Encrypt(value);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(
                                $"PII Encryption failed: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"PII Config processing failed: {ex.Message}");
            }

            //----------------------------------------
            // STEP 2: Try Audit Preparation
            //----------------------------------------
            try
            {
                foreach (var entry in ChangeTracker.Entries())
                {
                    if (entry.Entity is AuditLog ||
                        entry.State == EntityState.Detached ||
                        entry.State == EntityState.Unchanged)
                        continue;

                    var audit = AuditHelper.CreateAuditLog(
                        entry,
                        _userContext?.UserId,
                        _userContext?.TenantId);

                    if (audit != null)
                        auditEntries.Add(audit);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Audit preparation failed: {ex.Message}");
            }

            //----------------------------------------
            // STEP 3: Save Main Business Data (mandatory)
            //----------------------------------------
            var result = await base.SaveChangesAsync(cancellationToken);

            //----------------------------------------
            // STEP 4: Save Audit Logs separately
            //----------------------------------------
            if (auditEntries.Any())
            {
                try
                {
                    AuditLogs.AddRange(auditEntries);
                    await base.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Audit save failed: {ex.Message}");
                }
            }

            return result;
        }

        #endregion





        private List<PIIConfig> _piiConfigCache = new();
        private void LoadPIIConfigCache()
        {
            if (_piiLoaded)
                return;

            try
            {
                if (_piiConfigCache.Any())
                    return;

                _piiConfigCache = PIIConfigs
                    .AsNoTracking()
                    .Where(x => x.IsActive)
                    .ToList();

                _piiLoaded = true;
            }
            catch
            {
                _piiConfigCache = new List<PIIConfig>();
            }
        }
        private void OnEntityTracked(
            object sender,
            EntityTrackedEventArgs e)
        {
            if (!e.FromQuery || e.Entry.Entity == null)
                return;

            try
            {
                var tableName = e.Entry.Metadata.GetTableName();

                var piiConfigs = _piiConfigCache
                    .Where(x =>
                        x.TableName == tableName &&
                        (
                            x.TenantId == null ||
                            x.TenantId == _userContext?.TenantId
                        ))
                    .ToList();

                if (!piiConfigs.Any())
                    return;

                foreach (var config in piiConfigs)
                {
                    var property = e.Entry.Properties
                        .FirstOrDefault(x =>
                            x.Metadata.Name == config.ColumnName);

                    if (property?.CurrentValue == null)
                        continue;

                    var encryptedValue =
                        property.CurrentValue.ToString();

                    if (string.IsNullOrWhiteSpace(encryptedValue))
                        continue;

                    if (!EncryptionHelper.IsEncrypted(encryptedValue))
                        continue;

                    try
                    {
                        property.CurrentValue =
                            EncryptionHelper.Decrypt(encryptedValue);
                    }
                    catch
                    {
                        // ignore decrypt failures
                    }
                }
            }
            catch
            {
                // don't break query execution
            }
        }
    }
}