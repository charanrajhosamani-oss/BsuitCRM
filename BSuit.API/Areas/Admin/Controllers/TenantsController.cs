using BSuit.API.Areas.Admin.Controllers.Base;
using BSuit.API.Areas.Admin.Models;
using BSuit.Contracts.Services;
using BSuit.Core.Data;
using BSuit.Core.Entities;
using BSuit.Identity;
using BSuit.Identity.Models;
using BSuit.SalesCRM.Data;
using BSuit.SalesCRM.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BSuit.API.Areas.Admin.Controllers
{
    public class TenantsController : BaseAdminController
    {
        private readonly CoreDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IUserContext _userContext;

        public TenantsController(
            CoreDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IUserContext userContext)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _userContext = userContext;
        }

        // ================= INDEX =================
        public async Task<IActionResult> MyTenants(Guid id)
        {
            TenantVM model = new TenantVM();

            var query = _context.Tenants.AsQueryable();

            query = query.Where(x => x.Id == id);


            // SORT
            query = model.SortColumn switch
            {
                "Name" => model.SortDirection == "asc"
                    ? query.OrderBy(x => x.Name)
                    : query.OrderByDescending(x => x.Name),

                _ => query.OrderByDescending(x => x.CreatedOn)
            };

            model.TotalCount = await query.CountAsync();

            // PAGINATION
            model.Items = await query
                .Skip((model.PageNumber - 1) * model.PageSize)
                .Take(model.PageSize)
                .ToListAsync();

            // ✅ Pass roles to the View
            ViewBag.Roles = await _roleManager.Roles
                                              .OrderBy(r => r.Name)
                                             .Select(r => new { r.Name, r.NormalizedName })
                                             .ToListAsync();
            // ✅ Fetch tenant IDs that already have Identity users
            var userTenantIds = await _userManager.Users
                                    .Select(u => u.TenantId)
                                    .ToListAsync();

            ViewBag.TenantHasLogin = userTenantIds;

            return View(model);
        }

        public async Task<IActionResult> Index(TenantVM model)
        {
            var query = _context.Tenants.AsQueryable();

            if(_userContext.IsSuperAdmin == false)
            {
                query = query.Where(x =>
                  x.Id == _userContext.TenantId);
            }

            // SEARCH
            if (!string.IsNullOrEmpty(model.Search))
            {
                query = query.Where(x =>
                    x.Name.Contains(model.Search));
            }

            // SORT
            query = model.SortColumn switch
            {
                "Name" => model.SortDirection == "asc"
                    ? query.OrderBy(x => x.Name)
                    : query.OrderByDescending(x => x.Name),

                _ => query.OrderByDescending(x => x.CreatedOn)
            };

            model.TotalCount = await query.CountAsync();

            // PAGINATION
            model.Items = await query
                .Skip((model.PageNumber - 1) * model.PageSize)
                .Take(model.PageSize)
                .ToListAsync();

            // ✅ Pass roles to the View
            ViewBag.Roles = await _roleManager.Roles
                                              .OrderBy(r => r.Name)
                                             .Select(r => new { r.Name, r.NormalizedName })
                                             .ToListAsync();
            // ✅ Fetch tenant IDs that already have Identity users
            var userTenantIds = await _userManager.Users
                                    .Select(u => u.TenantId)
                                    .ToListAsync();

            ViewBag.TenantHasLogin = userTenantIds;

            return View(model);
        }

        // ================= CREATE =================
        [HttpPost]
        public async Task<IActionResult> Create(TenantMaster t)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Failed to create.";
                return RedirectToAction(nameof(Index));
            }

            var existing = await _context.Tenants.AnyAsync(e => e.Email == t.Email || e.Domain == t.Domain);

            if (!existing)
            {
                _context.Tenants.Add(t);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Tenant created successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        // ================= EDIT =================
        [HttpPost]
        public async Task<IActionResult> Edit(TenantMaster t)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Failed to update.";
                return RedirectToAction(nameof(Index));
            }
            var existing = await _context.Tenants.FindAsync(t.Id);

            if (existing == null)
                return NotFound();

            existing.Name = t.Name;
            existing.Domain = t.Domain;
            existing.Email = t.Email;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Tenant updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ================= TOGGLE =================
        [HttpPost]
        public async Task<IActionResult> Toggle(Guid id)
        {
            var t = await _context.Tenants.FindAsync(id);
            if (t == null)
            {
                TempData["Error"] = "Failed to update status.";
                return NotFound();
            }

            t.IsActive = !t.IsActive;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Tenant status changed successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ================= DELETE =================
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var t = await _context.Tenants.FindAsync(id);

            if (t == null)
            {
                TempData["Error"] = "Failed to delete Tenant.";
                return NotFound();
            }
            _context.Tenants.Remove(t);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Tenant deleted successfully.";
            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        public async Task<IActionResult> CreateLogin(TenantLoginVM model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Failed to create.";
                return RedirectToAction(nameof(Index));
            }

            //Check user exists
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                TempData["Error"] = "User already exists";
                return RedirectToAction(nameof(Index));
            }

            //Ensure Role exists
            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                var appRole = new ApplicationRole();
                appRole.IsActive = true;
                appRole.Name = model.Role;
                await _roleManager.CreateAsync(appRole);
            }

            //Create user
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                TenantId = model.TenantId,
                FullName = model.FullName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                TempData["Error"] = string.Join(",", result.Errors.Select(x => x.Description));
                return RedirectToAction(nameof(Index));
            }

            //Assign role
            await _userManager.AddToRoleAsync(user, model.Role);

            TempData["Success"] = "Login created successfully";

            return RedirectToAction(nameof(Index));
        }



    }
}