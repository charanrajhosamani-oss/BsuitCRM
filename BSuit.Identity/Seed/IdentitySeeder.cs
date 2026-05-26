
using BSuit.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BSuit.Identity.Seed
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services, Guid tenantId)
        {
            using var scope = services.CreateScope();

            var serviceProvider = scope.ServiceProvider;

            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();


            // ============================
            // 1. SEED ROLES
            // ============================
            await SeedRolesAsync(roleManager);


            // ============================
            // 2. SEED SUPER ADMIN
            // ============================
            await SeedSuperAdminAsync(userManager, tenantId);


            // ============================
            // 3. SEED ADMIN
            // ============================
            await SeedAdminAsync(userManager, tenantId);
        }

        // ============================
        // ROLES
        // ============================
        private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
        {
            var roles = AppRoles.All;

            foreach (var roleName in roles)
            {
                var exists = await roleManager.RoleExistsAsync(roleName);

                if (!exists)
                {
                    var role = new ApplicationRole
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpper(),
                        CreatedByUserId = "-1",
                        Description = roleName,
                        IsSystemRole = true,
                        IsActive = true,
                        TenantId = Guid.Empty
                    };

                    await roleManager.CreateAsync(role);
                }
            }
        }

        // ============================
        // SUPER ADMIN
        // ============================
        private static async Task SeedSuperAdminAsync(
            UserManager<ApplicationUser> userManager,
            Guid tenantId)
        {
            var fullName = "super admin";
            var email = "superadmin@bsuit.com";
            var roleName = AppRoles.SUPERADMIN;

            var user = await userManager.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    TenantId = tenantId,
                    FullName = fullName,
                    IsSuperAdmin = true,
                    IsActive = true,
                    SelectedRole = roleName,
                };

                await userManager.CreateAsync(user, $"SuperAdmin@{DateTime.UtcNow.Year}");
            }

            var isInRole = await userManager.IsInRoleAsync(user, roleName);

            if (!isInRole)
            {
                await userManager.AddToRoleAsync(user, roleName);
            }
        }

        private static async Task SeedAdminAsync(
           UserManager<ApplicationUser> userManager,
           Guid tenantId)
        {
            var fullName = "Admin";
            var email = "admin@bsuit.com";
            var roleName = AppRoles.ADMIN;

            var user = await userManager.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    TenantId = tenantId,
                    FullName = fullName,
                    IsSuperAdmin = true,
                    IsActive = true,
                    SelectedRole = roleName,
                };

                await userManager.CreateAsync(user, $"Admin@{DateTime.UtcNow.Year}");
            }

            var isInRole = await userManager.IsInRoleAsync(user, roleName);

            if (!isInRole)
            {
                await userManager.AddToRoleAsync(user, roleName);
            }
        }

    }
}