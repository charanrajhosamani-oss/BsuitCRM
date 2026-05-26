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
    [Area("Admin")]
    public class RolesController : BaseAdminController
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly CoreDbContext _context;
        private readonly IUserContext _userContext;

        public RolesController(
            RoleManager<ApplicationRole> roleManager,
            CoreDbContext context,
            IUserContext userContext)
        {
            _roleManager = roleManager;
            _context = context;
            _userContext = userContext;
        }

        // ================= INDEX =================
        public async Task<IActionResult> Index(RoleVM vm)
        {
            var query = _roleManager.Roles.AsQueryable();

            // Tenant filter
            if (!_userContext.IsSuperAdmin)
                query = query.Where(x => (x.TenantId == _userContext.TenantId || x.IsSystemRole == true) && !x.Name.Contains("admin"));
            
            // Search
            if (!string.IsNullOrWhiteSpace(vm.Search))
                query = query.Where(x => x.Name.Contains(vm.Search));

            vm.TotalCount = await query.CountAsync();

            var items = await query
                 .OrderByDescending(x => x.IsSystemRole)   // System roles first
                 .ThenBy(x => x.Name)                      // Then sort alphabetically
                 .Skip((vm.PageNumber - 1) * vm.PageSize)
                 .Take(vm.PageSize)
                 .Select(x => new RoleVM
                 {
                     Id = x.Id,
                     Name = x.Name,
                     Description = x.Description,
                     IsActive = x.IsActive,
                     TenantId = x.TenantId,
                     IsSystemRole = x.IsSystemRole, // optional if needed in UI

                     CanEdit = _userContext.IsSuperAdmin ||
                              (!x.IsSystemRole && x.TenantId == _userContext.TenantId)
                 })
                 .ToListAsync();

            // Load Tenants
            if (_userContext.IsSuperAdmin)
            {
                vm.Tenants = await _context.Tenants
                    .Select(t => new TenantDropdownVM
                    {
                        Id = t.Id,
                        Name = t.Name
                    }).ToListAsync();
            }
            else
            {
                vm.Tenants = await _context.Tenants
                    .Where(t => t.Id == _userContext.TenantId)
                    .Select(t => new TenantDropdownVM
                    {
                        Id = t.Id,
                        Name = t.Name
                    }).ToListAsync();

                vm.TenantId = _userContext.TenantId ?? Guid.Empty;
            }

            // Safe selection
            if (vm.TenantId == Guid.Empty && vm.Tenants.Any())
                vm.TenantId = vm.Tenants.First().Id;

            var page = new RolePageVM
            {
                Grid = vm,
                Items = items
            };

            return View(page);
        }

        // ================= SAVE =================
        [HttpPost]
        public async Task<IActionResult> Save(RolePageVM page)
        {
            var vm = page.Grid;

            if (vm == null)
                return RedirectToAction(nameof(Index));

            Guid tenantId = _userContext.IsSuperAdmin
                ? vm.TenantId
                : _userContext.TenantId.Value;

            var normalizedName = vm.Name.Trim().ToUpper();

            // Check duplicate within tenant only
            bool roleExists = await _roleManager.Roles
                .AnyAsync(x =>
                    x.TenantId == tenantId &&
                    x.NormalizedName == normalizedName &&
                    x.Id != vm.Id);

            if (roleExists)
            {
                TempData["Error"] = "Role already exists for this tenant.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrEmpty(vm.Id))
            {
                //tenant-created roles belong only to that tenant
                bool isSystemRole = false;
                string createdBy = tenantId.ToString();

                //Role:SuperAdmin    Tenant: NULL / Global Tenant IsSystemRole:true
                if (_userContext.IsSuperAdmin)
                {
                    isSystemRole = true;
                    createdBy = "-1";
                }

                var role = new ApplicationRole
                {
                    Name = vm.Name.Trim(),
                    NormalizedName = normalizedName,
                    Description = vm.Description,
                    TenantId = tenantId,
                    IsActive = true,
                    IsSystemRole = isSystemRole,
                    CreatedByUserId = createdBy
                };

                var result = await _roleManager.CreateAsync(role);

                if (!result.Succeeded)
                {
                    TempData["Error"] = string.Join(",",
                        result.Errors.Select(x => x.Description));

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Success"] = "Role created successfully.";
                }
            }
            else
            {
                var role = await _roleManager.FindByIdAsync(vm.Id);

                if (role == null)
                    return RedirectToAction(nameof(Index));

                if (!_userContext.IsSuperAdmin &&
                    role.TenantId != _userContext.TenantId)
                    return Unauthorized();

                role.Name = vm.Name.Trim();
                role.NormalizedName = normalizedName;
                role.Description = vm.Description;

                if (_userContext.IsSuperAdmin)
                    role.TenantId = vm.TenantId;

                var result = await _roleManager.UpdateAsync(role);

                if (!result.Succeeded)
                {
                    TempData["Error"] = string.Join(",",
                        result.Errors.Select(x => x.Description));

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Success"] = "Role saved successfully.";
                }
            }

            return RedirectToAction(nameof(Index));
        }


        // ================= DELETE =================
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
                return RedirectToAction(nameof(Index));

            if (!_userContext.IsSuperAdmin &&
                role.TenantId != _userContext.TenantId)
                return Unauthorized();

            await _roleManager.DeleteAsync(role);
            TempData["Success"] = "Role removed successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ================= TOGGLE =================
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
                return Json(new { success = false });

            if (!_userContext.IsSuperAdmin &&
                role.TenantId != _userContext.TenantId)
                return Json(new { success = false });

            role.IsActive = !role.IsActive;
            await _roleManager.UpdateAsync(role);
            TempData["Success"] = "Role status changed successfully.";
            return Json(new
            {
                success = true,
                status = role.IsActive
            });
        }
    }
}