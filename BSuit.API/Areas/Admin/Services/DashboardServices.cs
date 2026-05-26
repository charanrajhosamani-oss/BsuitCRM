using BSuit.API.Areas.Admin.Models;
using BSuit.API.Infrastructure.Services;
using BSuit.Contracts.Services;
using BSuit.Core.Data;
using BSuit.Identity;
using BSuit.Identity.Data;
using BSuit.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BSuit.API.Areas.Admin.Services
{
    public interface IDashboardService
    {
        Task<DashboardVM> GetDashboardAsync(ClaimsPrincipal user);
    }

    public class DashboardService : IDashboardService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly CoreDbContext _context;
        private readonly IdentityDbContext _identityContext;
        private readonly IUserContext _userContext;
        public DashboardService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            CoreDbContext context, IdentityDbContext identityContext,
            IUserContext userContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _identityContext = identityContext;
            _userContext = userContext;
        }

        private List<string> GetCurrentUserRoleIds(ApplicationUser user)
        {
            var roleNames = _userManager.GetRolesAsync(user).Result;

            return _roleManager.Roles
                .Where(x => roleNames.Contains(x.Name))
                .Select(x => x.Id)
                .ToList();
        }

       

        public async Task<DashboardVM> GetDashboardAsync(ClaimsPrincipal user)
        {
            var vm = new DashboardVM();

            var currentUser = await _userManager.GetUserAsync(user);
            if (currentUser == null)
                return vm;

            bool isSuperAdmin = await _userManager.IsInRoleAsync(currentUser, AppRoles.SUPERADMIN);
            bool isAdmin = await _userManager.IsInRoleAsync(currentUser, AppRoles.ADMIN);

            var tenantId = currentUser.TenantId;

            // =========================
            // BASE QUERIES
            // =========================
            var usersQuery = _userManager.Users.AsQueryable();
            var rolesQuery = _roleManager.Roles.AsQueryable();
            var menusQuery = _context.Menus.AsQueryable();
            var tenantsQuery = _context.Tenants.AsQueryable();

            // =========================
            // SUPERADMIN BYPASS
            // =========================
            if (!isSuperAdmin)
            {
                usersQuery = usersQuery.Where(x => x.TenantId == tenantId);
                rolesQuery = rolesQuery.Where(x => x.TenantId == tenantId);
                tenantsQuery = tenantsQuery.Where(x => x.Id == tenantId);

                var roleIdsAsString = GetCurrentUserRoleIds(currentUser);

                menusQuery = _context.RoleMenus
                    .Where(r => roleIdsAsString.Contains(r.RoleId) && r.IsActive)
                    .Select(r => r.Menu)
                    .Where(m => m.IsActive)
                    .OrderBy(m => m.SortOrder)
                    .AsQueryable();
            }


            // =========================
            // ADMIN RESTRICTION
            // =========================
            if (!isAdmin && !isSuperAdmin)
            {
                rolesQuery = rolesQuery.Where(x => !x.Name.Contains("ADMIN"));
            }

            // =========================
            // DASHBOARD COUNTS
            // =========================
            vm.TotalUsers = await usersQuery.CountAsync();
            vm.TotalRoles = await rolesQuery.CountAsync();
            vm.TotalMenus = await menusQuery.CountAsync();

            vm.TotalTenants = isSuperAdmin
                ? await tenantsQuery.CountAsync()
                : 1;

            // =========================
            // WEEKLY USER GRAPH (LAST 7 DAYS)
            // =========================
            var last7Days = Enumerable.Range(0, 7)
                .Select(i => DateTime.UtcNow.AddDays(-i))
                .OrderBy(d => d)
                .ToList();

            foreach (var day in last7Days)
            {
                var count = await usersQuery
                    .Where(x => x.CreatedOn.Date == day.Date)
                    .CountAsync();

                vm.WeeklyUsers.Add(new ChartItemVM
                {
                    Label = day.ToString("ddd"),
                    Value = count
                });
            }

            // =========================
            // RECENT ACTIVITY
            // =========================
            vm.RecentActivities = await usersQuery
                .OrderByDescending(x => x.CreatedOn)
                .Take(5)
                .Select(x => new ActivityVM
                {
                    Title = "User Created",
                    Description = x.FullName + " registered",
                    CreatedOn = x.CreatedOn,
                    Type = "CREATE",
                    UserName = x.FullName
                })
                .ToListAsync();



            vm.RolesWithUsers = await rolesQuery.Select(x=> new ChartItemVM
            { 
                 Label = x.IsSystemRole == true? $"{x.Name} (IN-BUILT)": x.Name,//RoleName
                 Value = _identityContext.UserRoles.Count(r=>r.RoleId == x.Id),
            }).ToListAsync();


            return vm;
        }
    }
}