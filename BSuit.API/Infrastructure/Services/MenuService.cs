using BSuit.API.Models;
using BSuit.Core.Data;
using BSuit.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace BSuit.API.Infrastructure.Services
{  
    public class MenuService
    {
        private readonly CoreDbContext _context;

        public MenuService(CoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<MenuItemVM>> GetMenuAsync(string roleId, bool isSuperAdmin, string currentUrl)
        {
            List<Menu> menus;

            if (isSuperAdmin)
            {
                menus = await _context.Menus                    
                    .OrderBy(x => x.SortOrder)
                    .ToListAsync();
            }
            else
            {
                menus = await _context.RoleMenus
                    .Where(r => r.RoleId == roleId && r.IsActive)
                    .Select(r => r.Menu)
                    .Where(m => m.IsActive)
                    .OrderBy(m => m.SortOrder)
                    .ToListAsync();
            }

            var list = menus.Select(m => new MenuItemVM
            {
                Id = m.Id,
                Name = m.Name,
                Url = m.Url,
                Icon = m.Icon,
                ParentId = m.ParentId,
                SortOrder = m.SortOrder
            }).ToList();

            var lookup = list.ToLookup(x => x.ParentId);

            foreach (var item in list)
            {
                item.Children = lookup[item.Id].OrderBy(x => x.SortOrder).ToList();
            }

            var roots = lookup[0].OrderBy(x => x.SortOrder).ToList();

            // 🔥 Mark Active
            MarkActive(roots, currentUrl.ToLower());

            return roots;
        }

        private bool MarkActive(List<MenuItemVM> menus, string currentUrl)
        {
            bool anyActive = false;

            foreach (var menu in menus)
            {
                if (!string.IsNullOrEmpty(menu.Url) &&
                    currentUrl.StartsWith(menu.Url.ToLower()))
                {
                    menu.IsActive = true;
                    anyActive = true;
                }

                if (menu.Children.Any())
                {
                    var childActive = MarkActive(menu.Children, currentUrl);

                    if (childActive)
                    {
                        menu.HasActiveChild = true;
                        anyActive = true;
                    }
                }
            }

            return anyActive;
        }

        // ================= BREADCRUMB =================
        public List<MenuItemVM> GetBreadcrumb(List<MenuItemVM> menus)
        {
            var path = new List<MenuItemVM>();

            FindPath(menus, path);

            return path;
        }

        private bool FindPath(List<MenuItemVM> menus, List<MenuItemVM> path)
        {
            foreach (var m in menus)
            {
                path.Add(m);

                if (m.IsActive)
                    return true;

                if (m.Children.Any() && FindPath(m.Children, path))
                    return true;

                path.Remove(m);
            }

            return false;
        }
    }
}
