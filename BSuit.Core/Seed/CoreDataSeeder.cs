using BSuit.Core.Data;
using BSuit.Core.Entities;
using BSuit.Identity.Models;
using BSuit.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Xml;

namespace BSuit.Core.Seed
{
    public static class CoreDataSeeder
    {
        public static async Task SeedAsync(CoreDbContext context)
        {
            await context.Database.MigrateAsync();

            // ✅ Seed Modules
            if (!await context.Modules.AnyAsync())
            {
                var modules = new List<ModuleMaster>
                {
                    new ModuleMaster { Name = "Sales CRM", Code = "SALESCRM" },
                    new ModuleMaster { Name = "HR", Code = "HR" },
                    new ModuleMaster { Name = "Admin", Code = "ADMIN" }
                };

                await context.Modules.AddRangeAsync(modules);
                await context.SaveChangesAsync();
            }

            // ✅ Seed Subscriptions
            if (!await context.Subscriptions.AnyAsync())
            {
                var subscriptions = new List<SubscriptionMaster>
                {
                    new SubscriptionMaster
                    {
                        Name = "Basic Plan",
                        Price = 0,
                        DurationInDays = 30,
                        IsActive = true
                    },
                    new SubscriptionMaster
                    {
                        Name = "Premium Plan",
                        Price = 999,
                        DurationInDays = 365,
                        IsActive = true
                    }
                };

                await context.Subscriptions.AddRangeAsync(subscriptions);
                await context.SaveChangesAsync();
            }

            // ✅ Seed Tenant
            if (!await context.Tenants.AnyAsync())
            {
                var tenant = new TenantMaster
                {
                    Name = "Tenant-001",
                    Domain = "brickworkindia",
                    Email = "rajeshkumar.p@brickworkindia.com",
                    IsActive = true
                };

                await context.Tenants.AddAsync(tenant);
                await context.SaveChangesAsync();



                //
                var encryptedKey = CryptoHelper.Generate256BitKey();
                var keyEntity = new EncryptionKey
                {
                    TenantId = tenant.Id,
                    EncryptedKey = encryptedKey,
                    Version = 1,
                    IsActive = true
                };

                context.EncryptionKeys.Add(keyEntity);
                await context.SaveChangesAsync();


                // ✅ Attach Subscription
                var subscription = await context.Subscriptions.FirstAsync();

                var tenantSubscription = new TenantSubscription
                {
                    TenantId = tenant.Id,
                    SubscriptionId = subscription.Id,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(subscription.DurationInDays),
                    IsActive = true
                };

                await context.TenantSubscriptions.AddAsync(tenantSubscription);
                await context.SaveChangesAsync();

                // ✅ Attach Modules to Subscription
                var modules = await context.Modules.ToListAsync();

                var tenantModules = modules.Select(m => new TenantSubscriptionModule
                {
                    TenantSubscriptionId = tenantSubscription.Id,
                    ModuleId = m.Id,
                    IsActive = true
                });

                await context.TenantSubscriptionModules.AddRangeAsync(tenantModules);
                await context.SaveChangesAsync();   
            }


            // ========================
            // MODULE MAPPING
            // ========================
            var existingModules = await context.TenantSubscriptionModules
                .ToListAsync();
            foreach (var module in existingModules)
            {
                if (!existingModules.Contains(module))
                {
                    context.TenantSubscriptionModules.Add(new TenantSubscriptionModule
                    {
                        TenantSubscriptionId = module.Id,
                        ModuleId = module.ModuleId,
                        IsActive = true
                    });
                }
            }
            await context.SaveChangesAsync();




        }
    }
}