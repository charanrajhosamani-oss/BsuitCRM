using BSuit.API.Areas.Admin.Controllers.Base;
using BSuit.API.Areas.Admin.Models;
using BSuit.Contracts.Services;
using BSuit.Core.Data;
using BSuit.Identity;
using BSuit.Identity.Models;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BSuit.API.Areas.Admin.Controllers
{
    public class UserPermissionController : BaseAdminController
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CoreDbContext _context;
        private readonly IUserContext _userContext;

        public UserPermissionController(CoreDbContext context,
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IUserContext userContext)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _userContext = userContext;
        }

        public async Task<IActionResult> Index(string userId)
        {
            var rolesQuery = _roleManager.Roles.AsQueryable();

            // 🔴 Exclude system roles
            rolesQuery = rolesQuery.Where(r =>
                r.Name != AppRoles.SUPERADMIN &&
                r.Name != AppRoles.ADMIN &&
                r.Name != AppRoles.TENANT
            );



            var vm = new UserPermissionVM();

            // 🔷 USERS (Tenant Filter)
            var usersQuery = _userManager.Users.AsQueryable();

            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.UserName == _userContext.UserName);

            if (!await _userManager.IsInRoleAsync(user, nameof(BSuit.Identity.AppRoles.ADMIN)) ||
                !await _userManager.IsInRoleAsync(user, nameof(BSuit.Identity.AppRoles.SUPERADMIN)))
            {
                usersQuery = usersQuery.Where(x => x.TenantId == _userContext.TenantId);
            }

            if (_userContext.IsSuperAdmin == true)
            {
                vm.Users = await usersQuery
                    .Select(u => new UserDropdownVM
                    {
                        Id = u.Id,
                        Name = u.FullName + " (" + u.TenantId + ")"
                    }).ToListAsync();
            }
            else
            {
                vm.Users = await usersQuery.Where(t=>t.TenantId == _userContext.TenantId)
                    .Select(u => new UserDropdownVM
                    {
                        Id = u.Id,
                        Name = u.FullName
                    }).ToListAsync();
            }
                vm.UserId = userId ?? vm.Users.FirstOrDefault()?.Id;

            // 🔷 Assigned permissions
            var assigned = await _context.UserPermissions
                .Where(x => x.UserId == vm.UserId)
                .ToListAsync();

            // 🔷 All permissions
            var permissions = await _context.PermissionMasters
                .Include(p => p.Module)
                .Where(p => p.IsActive)
                .ToListAsync();

            vm.PermissionGroups = permissions
                .GroupBy(p => p.Module.Name)
                .Select(g => new PermissionGroupVM
                {
                    ModuleName = g.Key,
                    Permissions = g.Select(p =>
                    {
                        var userPerm = assigned.FirstOrDefault(x => x.PermissionId == p.Id);

                        return new PermissionItemVM
                        {
                            Id = p.Id,
                            DisplayName = p.DisplayName,
                            IsAssigned = userPerm != null && userPerm.IsAllowed
                        };
                    }).ToList()
                }).ToList();

            // 🔷 GRID
            vm.AssignedPermissions = assigned
                .Select(x => new UserAssignedPermissionVM
                {
                    PermissionId = x.PermissionId,
                    ModuleName = permissions.First(p => p.Id == x.PermissionId).Module.Name,
                    DisplayName = permissions.First(p => p.Id == x.PermissionId).DisplayName,
                    IsAllowed = x.IsAllowed
                }).ToList();

            return View(vm);

        }
    }
}
