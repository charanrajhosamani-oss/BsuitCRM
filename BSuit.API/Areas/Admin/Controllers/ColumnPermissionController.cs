using BSuit.API.Areas.Admin.Controllers.Base;
using BSuit.API.Areas.Admin.Models;
using BSuit.Contracts.Services;
using BSuit.Core.Data;
using BSuit.Core.Entities;
using BSuit.Identity;
using BSuit.Identity.Data;
using BSuit.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;


namespace BSuit.API.Areas.Admin.Controllers
{
    public class ColumnPermissionController : BaseAdminController
    {

        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CoreDbContext _context;
        private readonly IdentityDbContext _identityContext;
        private readonly IUserContext _userContext;

        public ColumnPermissionController(CoreDbContext context,
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IUserContext userContext,
            IdentityDbContext identityContext)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _userContext = userContext;
            _identityContext = identityContext;
        }


        [HttpGet]
        public IActionResult GetTablesList()
        {
            var tables = GetTables();

            return Json(tables);
        }

        [HttpGet]
        public IActionResult GetColumnsByTable(string table)
        {
            var cols = GetColumns(table);

            return Json(cols);
        }


        public List<string> GetTables()
        {
            return _context.Model.GetEntityTypes()
                .Where(e =>
                    e.ClrType != null &&
                    !string.IsNullOrEmpty(e.GetTableName()) &&
                    !e.GetTableName().StartsWith("AspNet") &&
                    !e.GetTableName().StartsWith("__"))
                .Select(e => e.GetTableName())
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }

        public List<string> GetColumns(string tableName)
        {
            var entity = _context.Model.GetEntityTypes()
                .FirstOrDefault(e => e.GetTableName() == tableName);

            if (entity == null) return new List<string>();

            return entity.GetProperties()
                .Select(p => p.Name)
                .Where(c =>
                    c != "Id" &&
                    !c.EndsWith("Id") &&      // optional FK skip
                    c != "CreatedOn" &&
                    c != "UpdatedOn" &&
                    c != "IsActive" &&
                    c != "CreatedBy" &&
                    c != "UpdatedBy")
                .ToList();
        }

        public async Task<IActionResult> Index(string roleId, string userId)
        {
            var vm = new ColumnPermissionVM();

            var currentUser = await _userManager.Users
                .FirstOrDefaultAsync(x => x.UserName == _userContext.UserName);

            bool isAdmin =
                await _userManager.IsInRoleAsync(currentUser, nameof(BSuit.Identity.AppRoles.ADMIN));

            bool isSuperAdmin =
                await _userManager.IsInRoleAsync(currentUser, nameof(BSuit.Identity.AppRoles.SUPERADMIN));

            // ================= ROLES =================

            var rolesQuery = _roleManager.Roles.AsQueryable();

            if (!isAdmin && !isSuperAdmin)
            {
                rolesQuery = rolesQuery
                    .Where(x => x.TenantId == _userContext.TenantId);
            }

            vm.Roles = await rolesQuery
                .OrderBy(x => x.Name)
                .Select(x => new RoleDropdownVM
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();

            vm.RoleId = roleId ?? vm.Roles.FirstOrDefault()?.Id;

            // ================= USERS =================

            var usersQuery = _userManager.Users
                .Where(x => x.IsActive)
                .AsQueryable();

            if (!isAdmin && !isSuperAdmin)
            {                
                usersQuery = usersQuery
                    .Where(x => x.TenantId == _userContext.TenantId);
            }

            if (!string.IsNullOrEmpty(vm.RoleId))
            {
                var selectedRole = await _roleManager.FindByIdAsync(vm.RoleId);

                if (selectedRole != null)
                {
                    var roleIds = await _roleManager.Roles
                        .Where(x =>
                            x.Name == AppRoles.ADMIN ||
                            x.Name == AppRoles.SUPERADMIN)
                        .Select(x => x.Id)
                        .ToListAsync();

                    var userIds = await _identityContext.UserRoles
                        .Where(x => roleIds.Contains(x.RoleId))
                        .Select(x => x.UserId)
                        .Distinct()
                        .ToListAsync();

                    usersQuery = usersQuery
                        .Where(x => userIds.Contains(x.Id));
                }
            }

            vm.Users = await usersQuery
                .OrderBy(x => x.Email)
                .Select(x => new UserDropdownVM
                {
                    Id = x.Id,
                    Name = x.Email
                })
                .ToListAsync();

            vm.UserId = userId;

            // ================= LOOKUP MAPS =================

            var tenantMap = await _context.Tenants
                .ToDictionaryAsync(x => x.Id, x => x.Name);

            var permissionMap = await _context.PermissionMasters
                .ToDictionaryAsync(x => x.Id, x => x.Name);

            var userMap = await _userManager.Users
                .ToDictionaryAsync(x => x.Id, x => x.Email);

            // ================= PERMISSIONS =================

            List<ColumnPermission> assigned;

            if (!string.IsNullOrEmpty(vm.UserId))
            {
                assigned = await _context.ColumnPermissions
                    .Where(x => x.UserId == vm.UserId)
                    .ToListAsync();
            }
            else
            {
                assigned = await _context.ColumnPermissions
                    .Where(x => x.RoleId == vm.RoleId)
                    .ToListAsync();
            }

            vm.Assigned = assigned.Select(x => new ColumnAssignedVM
            {
                EntityName = x.EntityName,
                ColumnName = x.ColumnName,
                CanView = x.CanView,
                CanEdit = x.CanEdit,

                // Display Names instead of IDs
                TenantName = x.TenantId != Guid.Empty && tenantMap.ContainsKey(x.TenantId)
                    ? tenantMap[x.TenantId]
                    : "",

                PermissionName = x.PermissionId != 0 && permissionMap.ContainsKey(x.PermissionId)
                    ? permissionMap[x.PermissionId]
                    : "",

                UserName = !string.IsNullOrEmpty(x.UserId) && userMap.ContainsKey(x.UserId)
                    ? userMap[x.UserId]
                    : ""
            }).ToList();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(ColumnPermission model)
        {
            var tenantId = _userContext.TenantId;

            bool exists = await _context.ColumnPermissions.AnyAsync(x =>
                x.RoleId == model.RoleId &&
                x.UserId == model.UserId &&
                x.EntityName == model.EntityName &&
                x.ColumnName == model.ColumnName);

            if (!exists)
            {
                model.TenantId = tenantId.Value;

                _context.ColumnPermissions.Add(model);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", new
            {
                roleId = model.RoleId,
                userId = model.UserId
            });
        }


        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] ColumnPermission model)
        {
            var entity = await _context.ColumnPermissions
                .FirstOrDefaultAsync(x =>
                    x.RoleId == model.RoleId &&
                    x.UserId == model.UserId &&
                    x.EntityName == model.EntityName &&
                    x.ColumnName == model.ColumnName);

            if (entity != null)
            {
                _context.ColumnPermissions.Remove(entity);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }




    }
}