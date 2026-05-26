using BSuit.API.Areas.Admin.Controllers.Base;
using BSuit.API.Areas.Admin.Models;
using BSuit.Core.Data;
using BSuit.Core.Entities;
using BSuit.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BSuit.API.Areas.Admin.Controllers
{
    public class MenuController : BaseAdminController
    {
        private readonly CoreDbContext _context;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public MenuController(CoreDbContext context,
                              RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        // ================= INDEX =================
        public async Task<IActionResult> Index(MenuVM model)
        {
            var query = _context.Menus.AsQueryable();

            //SEARCH (Name + Url)
            if (!string.IsNullOrWhiteSpace(model.Search))
            {
                var search = model.Search.ToLower();

                query = query.Where(x =>
                    (x.Name ?? "").ToLower().Contains(search) ||
                    (x.Url ?? "").ToLower().Contains(search));
            }

            // 🔃 SORTING
            query = (model.SortColumn, model.SortDirection) switch
            {
                ("Name", "asc") => query.OrderBy(x => x.Name),
                ("Name", "desc") => query.OrderByDescending(x => x.Name),

                ("Url", "asc") => query.OrderBy(x => x.Url),
                ("Url", "desc") => query.OrderByDescending(x => x.Url),

                ("SortOrder", "asc") => query.OrderBy(x => x.SortOrder),
                ("SortOrder", "desc") => query.OrderByDescending(x => x.SortOrder),

                _ => query.OrderBy(x => x.SortOrder)
            };

            model.TotalCount = await query.CountAsync();

            model.Items = await query
                .Skip((model.PageNumber - 1) * model.PageSize)
                .Take(model.PageSize)
                .ToListAsync();

            return View(model);
        }



        // ================= CREATE =================
        [HttpPost]
        public async Task<IActionResult> Create(Menu m)
        {
            m.Icon ??= "";

            var url = (m.Url ?? "").Trim().ToLower();
            var name = (m.Name ?? "").Trim().ToLower();

            //DUPLICATE CHECK (URL OR NAME + PARENT)
            var exists = await _context.Menus.AnyAsync(x =>
                (x.Url ?? "").ToLower() == url
                ||
                (
                    (x.Name ?? "").ToLower() == name &&
                    x.ParentId == m.ParentId
                )
            );

            if (exists)
            {
                TempData["Error"] = "Menu already exists.";
                return RedirectToAction(nameof(Index));
            }

            _context.Menus.Add(m);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Menu created successfully";
            return RedirectToAction(nameof(Index));
        }

        // ================= EDIT =================
        [HttpPost]
        public async Task<IActionResult> Edit(Menu m)
        {
            var existing = await _context.Menus.FindAsync(m.Id);

            if (existing == null)
                return NotFound();

            var url = (m.Url ?? "").Trim().ToLower();
            var name = (m.Name ?? "").Trim().ToLower();

            //DUPLICATE CHECK (exclude self)
            var exists = await _context.Menus.AnyAsync(x =>
                x.Id != m.Id &&
                (
                    (x.Url ?? "").ToLower() == url
                    ||
                    (
                        (x.Name ?? "").ToLower() == name &&
                        x.ParentId == m.ParentId
                    )
                )
            );

            if (exists)
            {
                TempData["Error"] = "Duplicate menu exists.";
                return RedirectToAction(nameof(Index));
            }

            existing.Name = m.Name;
            existing.Url = m.Url;
            existing.Icon = m.Icon;
            existing.ParentId = m.ParentId;
            existing.SortOrder = m.SortOrder;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Menu updated";
            return RedirectToAction(nameof(Index));
        }

        // ================= TOGGLE =================
        [HttpPost]
        public async Task<IActionResult> Toggle(int id)
        {
            var item = await _context.Menus.FindAsync(id);

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
            var item = await _context.Menus.FindAsync(id);

            if (item == null)
                return NotFound();

            _context.Menus.Remove(item);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }











        // ================= ROLE-MENU MAPPING =================

        //LOAD UI
        public async Task<IActionResult> RoleMapping(RoleMenuMappingVM model)
        {
            var rolesQuery = _roleManager.Roles.AsQueryable();

            //SEARCH
            if (!string.IsNullOrWhiteSpace(model.Search))
            {
                var s = model.Search.ToLower();
                rolesQuery = rolesQuery.Where(x => x.Name.ToLower().Contains(s));
            }

            //SORT
            rolesQuery = (model.SortColumn, model.SortDirection) switch
            {
                ("Name", "asc") => rolesQuery.OrderBy(x => x.Name),
                ("Name", "desc") => rolesQuery.OrderByDescending(x => x.Name),
                _ => rolesQuery.OrderBy(x => x.Name)
            };

            model.TotalCount = await rolesQuery.CountAsync();

            model.Roles = await rolesQuery
                .Skip((model.PageNumber - 1) * model.PageSize)
                .Take(model.PageSize)
                .ToListAsync();

            //IF MENU ID PASSED → LOAD ONLY THAT MENU
            if (model.MenuId.HasValue && model.MenuId > 0)
            {
                var menu = await _context.Menus.FindAsync(model.MenuId.Value);

                model.MenuName = menu?.Name;

                model.AllMenus = await _context.Menus
                    .Where(x => x.Id == model.MenuId)
                    .ToListAsync();
            }
            else
            {
                //NORMAL MODE → ALL MENUS
                model.AllMenus = await _context.Menus
                    .Where(x => x.IsActive)
                    .OrderBy(x => x.SortOrder)
                    .ToListAsync();
            }

            //LOAD ASSIGNED MENUS
            if (!string.IsNullOrEmpty(model.SelectedRoleId))
            {
                var assignedQuery = _context.RoleMenus
                    .Where(x => x.RoleId == model.SelectedRoleId);

                if (model.MenuId.HasValue && model.MenuId > 0)
                {
                    assignedQuery = assignedQuery.Where(x => x.MenuId == model.MenuId);
                }

                model.AssignedMenuIds = await assignedQuery
                    .Select(x => x.MenuId)
                    .ToListAsync();
            }

            return View("RoleMapping", model);
        }

        [HttpPost]
        public async Task<IActionResult> RoleMapping(string SelectedRoleId, int MenuId)
        {
            if (string.IsNullOrEmpty(SelectedRoleId))
                return RedirectToAction(nameof(RoleMapping));

            if (MenuId == 0)
                return RedirectToAction(nameof(RoleMapping));


            var existing = _context.RoleMenus
                .Where(x => x.RoleId == SelectedRoleId);

            //IF MENU FILTER → REMOVE ONLY THAT MENU            
            existing = existing.Where(x => x.MenuId == MenuId);
            _context.RoleMenus.RemoveRange(existing);

            var menu = new RoleMenu
            {
                RoleId = SelectedRoleId,
                MenuId = MenuId
            };
            await _context.RoleMenus.AddAsync(menu);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Menus assigned successfully";
            return RedirectToAction(nameof(RoleMapping), new
            {
                SelectedRoleId = SelectedRoleId,
                MenuId = MenuId
            });
        }

        //SAVE MAPPING
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveRoleMapping(RoleMenuMappingVM model)
        {
            if (string.IsNullOrEmpty(model.SelectedRoleId))
            {
                TempData["Error"] = "Invalid role selection.";
                return RedirectToAction(nameof(RoleMapping), new
                {
                    SelectedRoleId = model.SelectedRoleId,
                    MenuId = model.MenuId,
                    Search = model.Search,
                    PageNumber = model.PageNumber,
                    SortColumn = model.SortColumn,
                    SortDirection = model.SortDirection
                });
            }

            if (!model.MenuId.HasValue)
            {
                TempData["Error"] = "Invalid menu selection.";
                return RedirectToAction(nameof(RoleMapping), new
                {
                    SelectedRoleId = model.SelectedRoleId,
                    MenuId = model.MenuId,
                    Search = model.Search,
                    PageNumber = model.PageNumber,
                    SortColumn = model.SortColumn,
                    SortDirection = model.SortDirection
                });
            }

            var existing = _context.RoleMenus
                .Where(x => x.RoleId == model.SelectedRoleId);

            //IF MENU FILTER → REMOVE ONLY THAT MENU
            if (model.MenuId.HasValue && model.MenuId > 0)
            {
                existing = existing.Where(x => x.MenuId == model.MenuId);
            }

            _context.RoleMenus.RemoveRange(existing);

            if (model.SelectedMenuIds != null && model.SelectedMenuIds.Any())
            {
                var list = model.SelectedMenuIds.Select(m => new RoleMenu
                {
                    RoleId = model.SelectedRoleId,
                    MenuId = m
                });

                await _context.RoleMenus.AddRangeAsync(list);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Menus assigned successfully";

            return RedirectToAction(nameof(RoleMapping), new
            {
                SelectedRoleId = model.SelectedRoleId,
                MenuId = model.MenuId,
                Search = model.Search,
                PageNumber = model.PageNumber,
                SortColumn = model.SortColumn,
                SortDirection = model.SortDirection
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAssignedMenus(string roleId, int? menuId)
        {
            if (string.IsNullOrEmpty(roleId))
                return Json(new List<int>());

            var query = _context.RoleMenus
                .Where(x => x.RoleId == roleId);

            if (menuId.HasValue && menuId > 0)
            {
                query = query.Where(x => x.MenuId == menuId);
            }

            var assignedMenuIds = await query
                .Select(x => x.MenuId)
                .ToListAsync();

            return Json(assignedMenuIds);
        }


    }
}