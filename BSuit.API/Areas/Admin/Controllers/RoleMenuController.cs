using BSuit.API.Areas.Admin.Controllers.Base;
using BSuit.Core.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BSuit.API.Areas.Admin.Controllers
{
    public class RoleMenuController : BaseAdminController
    {
        private readonly CoreDbContext _context;

        public RoleMenuController(CoreDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Manage(string roleId)
        {
            var menus = await _context.Menus.Where(x => x.IsActive).ToListAsync();

            var roleMenus = await _context.RoleMenus
                .Where(x => x.RoleId == roleId && x.IsActive)
                .ToListAsync();

            var model = menus.Select(m => new
            {
                m.Id,
                m.Name,
                IsSelected = roleMenus.Any(r => r.MenuId == m.Id)
            }).ToList();

            ViewBag.RoleId = roleId;
            return View(model);
        }
    }
}
