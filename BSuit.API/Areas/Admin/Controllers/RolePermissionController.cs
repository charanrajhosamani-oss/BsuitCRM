using BSuit.API.Areas.Admin.Controllers.Base;
using BSuit.API.Areas.Admin.Models;
using BSuit.Contracts.Services;
using BSuit.Core.Data;
using BSuit.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BSuit.API.Areas.Admin.Controllers
{
    public class RolePermissionController : BaseAdminController
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CoreDbContext _context;
        private readonly IUserContext _userContext;

        public RolePermissionController(CoreDbContext context,
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IUserContext userContext)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _userContext = userContext;
        }


        public async Task<IActionResult> Index(string roleId)
        {
            var vm = new RolePermissionVM();

            var rolesQuery = _roleManager.Roles.AsQueryable();

            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.UserName == _userContext.UserName);

            // Fix role filter logic
            if (!await _userManager.IsInRoleAsync(user, nameof(BSuit.Identity.AppRoles.SUPERADMIN)))
            {
                rolesQuery = rolesQuery.Where(x => x.TenantId == _userContext.TenantId);
            }

            // Step 1: Fetch roles first
            var roles = await rolesQuery
                .Select(r => new
                {
                    r.Id,
                    r.Name,
                    r.TenantId
                })
                .ToListAsync();

            // Step 2: Fetch tenant names separately
            var tenantIds = roles
                .Where(x => x.TenantId != null)
                .Select(x => x.TenantId)
                .Distinct()
                .ToList();

            var tenants = await _context.Tenants
                .Where(t => tenantIds.Contains(t.Id))
                .Select(t => new
                {
                    t.Id,
                    t.Name
                })
                .ToDictionaryAsync(x => x.Id, x => x.Name);

            // Step 3: Merge in memory
            var d = _userManager.IsInRoleAsync(user, nameof(BSuit.Identity.AppRoles.ADMIN)).Result;
            vm.Roles = roles
                .Select(r => new RoleDropdownVM
                {
                    Id = r.Id,
                    Name = (_userContext.IsSuperAdmin || d)
                        ? $"{r.Name} ({(tenants.ContainsKey(r.TenantId) ? tenants[r.TenantId] : "System Role")})"
                        : r.Name
                })
                .ToList();

            vm.RoleId = roleId ?? vm.Roles.FirstOrDefault()?.Id;

            // Assigned permissions
            var assigned = await _context.RolePermissions
                .Where(x => x.RoleId == vm.RoleId)
                .Select(x => x.PermissionId)
                .ToListAsync();

            var permissions = await _context.PermissionMasters
                .Include(p => p.Module)
                .Where(p => p.IsActive)
                .ToListAsync();

            vm.PermissionGroups = permissions
                .GroupBy(p => p.Module.Name)
                .Select(g => new PermissionGroupVM
                {
                    ModuleName = g.Key,
                    Permissions = g.Select(p => new PermissionItemVM
                    {
                        Id = p.Id,
                        Name = p.Name,
                        DisplayName = p.DisplayName,
                        IsAssigned = assigned.Contains(p.Id)
                    }).ToList()
                }).ToList();


            var roleDictionary = await _roleManager.Roles
                    .ToDictionaryAsync(x => x.Id, x => x.Name);

            var RolePermissions = await _context.RolePermissions
                .Where(x => x.RoleId == vm.RoleId)
                .Include(x => x.Permission)
                    .ThenInclude(p => p.Module)
                .ToListAsync();

            vm.AssignedPermissions = RolePermissions
                .Select(x => new AssignedPermissionVM
                {
                    PermissionId = x.PermissionId,
                    ModuleName = x.Permission.Module.Name,
                    DisplayName = x.Permission.DisplayName,
                    RoleName = roleDictionary.ContainsKey(x.RoleId)
                        ? roleDictionary[x.RoleId]
                        : ""
                })
                .ToList();

            return View(vm);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(string roleId, List<int> permissionIds)
        {
            var existing = _context.RolePermissions
                .Where(x => x.RoleId == roleId);

            _context.RolePermissions.RemoveRange(existing);

            var newMappings = permissionIds.Select(pid => new Core.Entities.RolePermission
            {
                RoleId = roleId,
                PermissionId = pid,
                TenantId = _userContext.TenantId
            });

            await _context.RolePermissions.AddRangeAsync(newMappings);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Permissions updated successfully.";
            return RedirectToAction("Index", new { roleId });
        }


        [HttpPost]
        public async Task<IActionResult> RemovePermission(string roleId, int permissionId)
        {
            var entity = await _context.RolePermissions
                .FirstOrDefaultAsync(x => x.RoleId == roleId && x.PermissionId == permissionId);

            if (entity != null)
            {
                _context.RolePermissions.Remove(entity);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Permission removed successfully.";
            }
            TempData["Error"] = "Failed to update permissions.";
            return Json(new { success = true });
        }



    }
}
