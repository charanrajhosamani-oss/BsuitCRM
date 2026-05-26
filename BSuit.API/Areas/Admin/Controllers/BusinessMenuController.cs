using BSuit.API.Areas.Admin.Models;
using BSuit.API.Infrastructure.Constants;
using BSuit.Core.Data;
using BSuit.Core.Entities;
using BSuit.Identity.Data;
using BSuit.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;



namespace BSuit.API.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BusinessMenuController : Controller
    {
        private readonly CoreDbContext _context;
        private readonly IdentityDbContext _identityContext;
        private readonly UserManager<ApplicationUser> _userManager;
        public BusinessMenuController(
            CoreDbContext context, IdentityDbContext identityContext,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _identityContext = identityContext;
        }
        // ================= INDEX =================
        public async Task<IActionResult> Index(
                     BusinessMenuMasterVM model,
                     string search,
                     int pageNumber = 1,
                     int pageSize = 10)
        {
            var (user, isSuperAdmin) =
                await GetCurrentUserAsync();

            model.IsSuperAdmin = isSuperAdmin;

            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
            model.Search = search;

            //------------------------------------
            // QUERY
            //------------------------------------

            var query = _context.BusinessMenuMasters
                .Include(x => x.Module)
                .Include(x => x.ParentMenu)
                .AsQueryable();

            //------------------------------------
            // TENANT FILTER
            //------------------------------------

            if (!isSuperAdmin)
            {
                query = query.Where(x =>
                    x.TenantId == user.TenantId);
            }
            else if (model.TenantId != Guid.Empty)
            {
                query = query.Where(x =>
                    x.TenantId == model.TenantId);
            }

            //------------------------------------
            // SEARCH
            //------------------------------------

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.MenuName.Contains(search) ||
                    x.Url.Contains(search) ||
                    x.Remarks.Contains(search));
            }

            //------------------------------------
            // SORTING
            //------------------------------------

            query = (model.SortColumn, model.SortDirection) switch
            {
                ("MenuName", "asc")
                    => query.OrderBy(x => x.MenuName),

                ("MenuName", "desc")
                    => query.OrderByDescending(x => x.MenuName),

                ("Url", "asc")
                    => query.OrderBy(x => x.Url),

                ("Url", "desc")
                    => query.OrderByDescending(x => x.Url),

                ("SortOrder", "asc")
                    => query.OrderBy(x => x.SortOrder),

                ("SortOrder", "desc")
                    => query.OrderByDescending(x => x.SortOrder),

                _ => query.OrderBy(x => x.SortOrder)
            };

            //------------------------------------
            // COUNT
            //------------------------------------

            model.TotalCount = await query.CountAsync();

            //------------------------------------
            // DATA
            //------------------------------------

            model.Items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            //------------------------------------
            // DEFAULT TENANT
            //------------------------------------

            if (!isSuperAdmin)
            {
                model.TenantId =
                    user.TenantId.GetValueOrDefault();
            }

            //------------------------------------
            // DROPDOWNS
            //------------------------------------

            await LoadDropdowns(
                model,
                model.TenantId);

            return View(model);
        }


        // ================= CREATE =================
        [HttpPost]
        public async Task<IActionResult> Create(BusinessMenuMaster m)
        {
            try
            {
                var (user, isSuperAdmin) =
                    await GetCurrentUserAsync();

                //------------------------------------
                // TENANT
                //------------------------------------

                if (!isSuperAdmin)
                {
                    m.TenantId =
                        user.TenantId.GetValueOrDefault();
                }

                //------------------------------------
                // ROOT MENU
                //------------------------------------

                if (!m.ParentMenuId.HasValue ||
                    m.ParentMenuId <= 0)
                {
                    m.ParentMenuId = null;
                }

                //------------------------------------
                // DUPLICATE CHECK
                //------------------------------------

                var url = (m.Url ?? "")
                    .Trim()
                    .ToLower();

                var name = (m.MenuName ?? "")
                    .Trim()
                    .ToLower();

                var exists =
                    await _context.BusinessMenuMasters
                    .AnyAsync(x =>
                        x.TenantId == m.TenantId
                        &&
                        (
                            (x.Url ?? "").ToLower() == url
                            ||
                            (
                                (x.MenuName ?? "").ToLower() == name
                                &&
                                x.ParentMenuId == m.ParentMenuId
                            )
                        ));

                if (exists)
                {
                    TempData["Error"] =
                        "Menu already exists.";

                    return RedirectToAction(nameof(Index));
                }

                //------------------------------------
                // SAVE
                //------------------------------------

                m.CreatedOn = DateTime.UtcNow;
                m.IsActive = true;
                m.Icon ??= "";
                m.Remarks = m.MenuName;

                _context.BusinessMenuMasters.Add(m);

                await _context.SaveChangesAsync();

                TempData["Success"] =
                    "Menu created successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // ================= EDIT =================
        [HttpPost]
        public async Task<IActionResult> Edit(BusinessMenuMaster m)
        {
            try
            {
                var existing =
                    await _context.BusinessMenuMasters
                    .FirstOrDefaultAsync(x => x.Id == m.Id);

                if (existing == null)
                {
                    TempData["Error"] = "Menu not found.";
                    return RedirectToAction(nameof(Index));
                }

                var (user, isSuperAdmin) =
                    await GetCurrentUserAsync();

                //------------------------------------
                // TENANT
                //------------------------------------

                if (isSuperAdmin)
                {
                    existing.TenantId = m.TenantId;
                }
                else
                {
                    existing.TenantId =
                        user.TenantId.GetValueOrDefault();
                }

                //------------------------------------
                // PARENT VALIDATION
                //------------------------------------

                if (!m.ParentMenuId.HasValue ||
                    m.ParentMenuId <= 0)
                {
                    existing.ParentMenuId = null;
                }
                else
                {
                    if (m.ParentMenuId == m.Id)
                    {
                        TempData["Error"] =
                            "Menu cannot be parent of itself.";

                        return RedirectToAction(nameof(Index));
                    }

                    existing.ParentMenuId =
                        m.ParentMenuId;
                }

                //------------------------------------
                // DUPLICATE CHECK
                //------------------------------------

                var url = (m.Url ?? "")
                    .Trim()
                    .ToLower();

                var name = (m.MenuName ?? "")
                    .Trim()
                    .ToLower();

                var exists =
                    await _context.BusinessMenuMasters
                    .AnyAsync(x =>
                        x.Id != m.Id
                        &&
                        x.TenantId == existing.TenantId
                        &&
                        (
                            (x.Url ?? "").ToLower() == url
                            ||
                            (
                                (x.MenuName ?? "").ToLower() == name
                                &&
                                x.ParentMenuId == m.ParentMenuId
                            )
                        ));

                if (exists)
                {
                    TempData["Error"] =
                        "Duplicate menu exists.";

                    return RedirectToAction(nameof(Index));
                }

                //------------------------------------
                // UPDATE
                //------------------------------------

                existing.MenuName = m.MenuName;
                existing.Url = m.Url;
                existing.Icon = m.Icon ?? "";
                existing.SortOrder = m.SortOrder;
                existing.Remarks = m.MenuName;

                await _context.SaveChangesAsync();

                TempData["Success"] =
                    "Menu updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // ================= TOGGLE =================
        [HttpPost]
        public async Task<IActionResult> Toggle(int id)
        {
            var item = await _context.BusinessMenuMasters.FindAsync(id);

            if (item == null)
                return NotFound();

            item.IsActive = !item.IsActive;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ================= DELETE =================
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.BusinessMenuMasters.FindAsync(id);

            if (item == null)
                return NotFound();

            _context.BusinessMenuMasters.Remove(item);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }




        private async Task LoadDropdowns(
            BusinessMenuMasterVM vm,
            Guid? tenantId)
        {
            vm.Modules = await _context.Modules
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToListAsync();

            vm.ParentMenus = await _context
                .BusinessMenuMasters
                .Where(x => x.TenantId == tenantId)
                .OrderBy(x => x.MenuName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.MenuName
                })
                .ToListAsync();

            vm.Tenants = await _context.Tenants
               .OrderBy(x => x.Email)
               .Select(x => new SelectListItem
               {
                   Value = x.Id.ToString(),
                   Text = x.Name
               })
               .ToListAsync();

            var user = await _userManager.GetUserAsync(User);
            var (_, isSuperAdmin) = await GetCurrentUserAsync();
            if (!isSuperAdmin)
            {
                vm.Tenants = vm.Tenants
                    .Where(t =>
                        t.Value == user.TenantId.ToString())
                    .ToList();
            }
        }



        private async Task<(ApplicationUser user, bool isSuperAdmin)> GetCurrentUserAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            var isSuperAdminClaim =
                User.FindFirst(PARAMS.ADMIN)?.Value;

            bool isSuperAdmin =
                bool.TryParse(isSuperAdminClaim, out var result)
                && result;

            return (user, isSuperAdmin);
        }







        #region ROLE MAPPING

        public async Task<IActionResult> RoleMapping(
            int menuId,
            string search = "",
            string sortColumn = "Name",
            string sortDirection = "asc",
            int pageNumber = 1,
            int pageSize = 10)
        {
            var vm = new BusinessRoleMenuMappingVM();

            vm.MenuId = menuId;
            vm.Search = search;
            vm.SortColumn = sortColumn;
            vm.SortDirection = sortDirection;
            vm.PageNumber = pageNumber;
            vm.PageSize = pageSize;

            //------------------------------------
            // MENU
            //------------------------------------

            var menu = await _context.BusinessMenuMasters
                .FirstOrDefaultAsync(x => x.Id == menuId);

            if (menu == null)
            {
                TempData["Error"] = "Menu not found.";
                return RedirectToAction("Index");
            }

            vm.MenuName = menu.MenuName;
            vm.TenantId = menu.TenantId;

            //------------------------------------
            // ROLES
            //------------------------------------

            var user = await _userManager.GetUserAsync(User);

            var query = _identityContext.Roles
                .AsQueryable();

            if (user?.IsSuperAdmin == false)
            {
                query = query.Where(r => r.TenantId == user.TenantId).AsQueryable();
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.Name.Contains(search));
            }

            //------------------------------------
            // SORTING
            //------------------------------------

            switch (sortColumn)
            {
                case "Name":

                    query = sortDirection == "asc"
                        ? query.OrderBy(x => x.Name)
                        : query.OrderByDescending(x => x.Name);

                    break;

                default:

                    query = query.OrderBy(x => x.Name);

                    break;
            }

            //------------------------------------
            // COUNT
            //------------------------------------

            vm.TotalCount = await query.CountAsync();

            //------------------------------------
            // PAGING
            //------------------------------------


            var roles = await query
                .Where(r => r.IsSystemRole == false)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();


            vm.Roles = roles
                .Select(x => new RoleItemVM
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .OrderBy(n => n.Name)
                .ToList();

            //------------------------------------
            // ASSIGNED ROLES
            //------------------------------------

            vm.AssignedRoleIds = await _context.BusinessRoleMenuMappings
                .Where(x => x.MenuId == menuId)
                .Select(x => x.RoleId)
                .ToListAsync();

            return View(vm);
        }

        #endregion

        #region SAVE ROLE MAPPING

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveRoleMapping(
            int menuId,
            string roleId)
        {
            try
            {
                //------------------------------------
                // CHECK EXISTING
                //------------------------------------

                var existing =
                    await _context.BusinessRoleMenuMappings
                    .FirstOrDefaultAsync(x =>
                        x.MenuId == menuId
                        && x.RoleId == roleId);

                //------------------------------------
                // REMOVE
                //------------------------------------

                if (existing != null)
                {
                    _context.BusinessRoleMenuMappings
                        .Remove(existing);

                    await _context.SaveChangesAsync();

                    TempData["Success"] =
                        "Role mapping removed successfully.";
                }
                else
                {
                    //------------------------------------
                    // MENU
                    //------------------------------------

                    var menu =
                        await _context.BusinessMenuMasters
                        .FirstOrDefaultAsync(x =>
                            x.Id == menuId);

                    if (menu == null)
                    {
                        TempData["Error"] =
                            "Menu not found.";

                        return RedirectToAction(
                            "RoleMapping",
                            new { menuId });
                    }

                    //------------------------------------
                    // ADD
                    //------------------------------------

                    var mapping =
                        new BusinessRoleMenuMapping
                        {
                            TenantId = menu.TenantId,
                            MenuId = menuId,
                            RoleId = roleId,
                            IsActive = true,
                            CreatedOn = DateTime.UtcNow,
                            Remarks=$"MenuId:{menuId},RoleId:{roleId},TenantId:{menu.TenantId}"
                        };

                    _context.BusinessRoleMenuMappings
                        .Add(mapping);

                    await _context.SaveChangesAsync();

                    TempData["Success"] =
                        "Role mapped successfully.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;
            }

            return RedirectToAction(
                "RoleMapping",
                new { menuId });
        }

        #endregion

    }
}