using BSuit.API.Areas.Admin.Controllers.Base;
using BSuit.API.Areas.Admin.Models;
using BSuit.Contracts.Services;
using BSuit.Core.Data;
using BSuit.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BSuit.API.Areas.Admin.Controllers
{
    public class TenantModulesController : BaseAdminController
    {
        private readonly CoreDbContext _context;
        private readonly IUserContext _userContext;
        public TenantModulesController(CoreDbContext context,
            IUserContext userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        // ================= LIST =================
        public async Task<IActionResult> Index(
            string search,
            int pageNumber = 1,
            int pageSize = 10,
            Guid? tenantId = null)
        {
            var query = _context.TenantSubscriptionModules
                .Include(x => x.TenantSubscription)
                    .ThenInclude(ts => ts.Tenant)
                .Include(x => x.Module)
                .AsQueryable();

            if (tenantId.HasValue && tenantId.Value != Guid.Empty)
            {
                query = query.Where(x =>
                    x.TenantSubscription.TenantId == tenantId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.TenantSubscription.Tenant.Name.Contains(search) ||
                    x.Module.Name.Contains(search));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.TenantSubscription.Tenant.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.Search = search;

            if (_userContext.IsSuperAdmin == true)
            {
                ViewBag.Tenants = await _context.Tenants
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToListAsync();
            }
            else
            {
                ViewBag.Tenants = await _context.Tenants.Where(t => t.Id == _userContext.TenantId)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToListAsync();
            }

            ViewBag.Modules = await _context.Modules
                    .Where(x => x.IsActive)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name
                    }).ToListAsync();

            return View(items);
        }

        // ================= ASSIGN MODULE =================
        [HttpPost]
        public async Task<IActionResult> Create(Guid tenantId, int moduleId)
        {
            var subscription = await _context.TenantSubscriptions
                .FirstOrDefaultAsync(x =>
                    x.TenantId == tenantId && x.IsActive);

            if (subscription == null)
            {
                TempData["Error"] = "Active subscription not found.";
                return RedirectToAction(nameof(Index));
            }

            bool exists = await _context.TenantSubscriptionModules
                .AnyAsync(x =>
                    x.TenantSubscriptionId == subscription.Id &&
                    x.ModuleId == moduleId);

            if (exists)
            {
                TempData["Error"] = "Module already assigned.";
                return RedirectToAction(nameof(Index));
            }

            _context.TenantSubscriptionModules.Add(
                new TenantSubscriptionModule
                {
                    TenantSubscriptionId = subscription.Id,
                    ModuleId = moduleId
                });

            await _context.SaveChangesAsync();

            TempData["Success"] = "Module assigned successfully.";
            return RedirectToAction(nameof(Index), new { tenantId = tenantId });
        }

        // ================= MANAGE =================
        public async Task<IActionResult> Manage(Guid tenantId)
        {
            var tenant = await _context.Tenants.FindAsync(tenantId);

            var modules = await _context.Modules
                .Where(x => x.IsActive)
                .ToListAsync();

            var assignedModules = await _context.TenantSubscriptionModules
                .Where(x => x.TenantSubscription.TenantId == tenantId)
                .Select(x => x.ModuleId)
                .ToListAsync();

            var vm = new TenantModuleVM
            {
                TenantId = tenantId,
                TenantName = tenant.Name,
                Modules = modules.Select(x => new ModuleCheckbox
                {
                    ModuleId = x.Id,
                    Name = x.Name,
                    IsSelected = assignedModules.Contains(x.Id)
                }).ToList()
            };

            return View(vm);
        }

        // ================= SAVE =================
        [HttpPost]
        public async Task<IActionResult> Manage(TenantModuleVM model)
        {
            var subscription = await _context.TenantSubscriptions
                .FirstOrDefaultAsync(x =>
                    x.TenantId == model.TenantId &&
                    x.IsActive);

            if (subscription == null)
            {
                TempData["Error"] = "Subscription not found.";
                return RedirectToAction(nameof(Index));
            }

            var existing = await _context.TenantSubscriptionModules
                .Where(x => x.TenantSubscriptionId == subscription.Id)
                .ToListAsync();

            // Remove unchecked modules
            var removeItems = existing
                .Where(x => !model.Modules
                    .Any(m => m.ModuleId == x.ModuleId && m.IsSelected))
                .ToList();

            _context.TenantSubscriptionModules.RemoveRange(removeItems);

            // Add newly checked modules
            var selectedIds = model.Modules
                .Where(x => x.IsSelected)
                .Select(x => x.ModuleId)
                .ToList();

            foreach (var moduleId in selectedIds)
            {
                if (!existing.Any(x => x.ModuleId == moduleId))
                {
                    _context.TenantSubscriptionModules.Add(
                        new TenantSubscriptionModule
                        {
                            TenantSubscriptionId = subscription.Id,
                            ModuleId = moduleId
                        });
                }
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Modules updated successfully.";
            return RedirectToAction(nameof(Index), new { tenantId = model .TenantId});
        }

        // ================= SINGLE DELETE =================
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _context.TenantSubscriptionModules
                            .Include(x => x.TenantSubscription)
                            .FirstOrDefaultAsync(x => x.Id == id);

            Guid tenantId = Guid.Empty;

            if (entity != null)
            {
                tenantId = entity.TenantSubscription.TenantId;

                _context.TenantSubscriptionModules.Remove(entity);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Module deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Module not found.";
            }

            
            return RedirectToAction(nameof(Index), new { tenantId = tenantId });
        }

        // ================= BULK DELETE =================
        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<Guid> ids)
        {
            var items = await _context.TenantSubscriptionModules
                .Where(x => ids.Contains(x.Id))
                .ToListAsync();

            var tenantId = items.FirstOrDefault()?.TenantSubscription.TenantId;

            _context.TenantSubscriptionModules.RemoveRange(items);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Selected modules deleted.";
            return RedirectToAction(nameof(Index), new { tenantId = tenantId });
        }
    }
}