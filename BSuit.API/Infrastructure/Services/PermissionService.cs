using BSuit.Contracts.Services;
using BSuit.Core.Data;
using BSuit.Identity;
using BSuit.Identity.Data;
using BSuit.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BSuit.API.Infrastructure.Services
{
    public interface IPermissionService
    {
        Task<bool> HasPermissionAsync(string permissionName);

        Task<bool> CanViewColumnAsync(
            string entityName,
            string columnName,
            string permissionName);

        Task<bool> CanEditColumnAsync(
            string entityName,
            string columnName,
            string permissionName);

        Task<bool> HasMenuAccessAsync(int menuId);
    }
}


/*
 
@inject BSuit.API.Infrastructure.Services.IPermissionService PermissionService

@if (await PermissionService.HasPermissionAsync("Customer_Create"))
{
    <button class="btn btn-success">
        Add Customer
    </button>
}

@if (await PermissionService.CanViewColumnAsync(
    "Customer",
    "Salary",
    "Customer_View"))
{
    <td>@item.Salary</td>
}

 */

namespace BSuit.API.Infrastructure.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class PermissionService : IPermissionService
    {
        private readonly CoreDbContext _context;
        private readonly IdentityDbContext _identityContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserContext _userContext;

        public PermissionService(
            CoreDbContext context,
            UserManager<ApplicationUser> userManager,
            IUserContext userContext,
            IdentityDbContext identityContext)
        {
            _context = context;
            _userManager = userManager;
            _userContext = userContext;
            _identityContext = identityContext;
        }

        public async Task<bool> HasPermissionAsync(string permissionName)
        {
            var user = await GetCurrentUser();

            if (user == null)
                return false;

            if (await IsSystemAdmin(user))
                return true;

            var permission = await _context.PermissionMasters
                .FirstOrDefaultAsync(x => x.DisplayName == permissionName);

            if (permission == null)
                return false;

            // STEP 1 → USER OVERRIDE
            var userPermission = await _context.UserPermissions
                .Where(x =>
                    x.UserId == user.Id &&
                    x.PermissionId == permission.Id)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            if (userPermission != null)
                return userPermission.IsAllowed;

            // STEP 2 → ROLE BASE
            var roleIds = await GetCurrentUserRoleIds(user);

            var rolePermission = await _context.RolePermissions
                .AnyAsync(x =>
                    roleIds.Contains(x.RoleId) &&
                    x.PermissionId == permission.Id);

            return rolePermission;
        }

        public async Task<bool> HasMenuAccessAsync(int menuId)
        {
            var user = await GetCurrentUser();

            if (user == null)
                return false;

            if (await IsSystemAdmin(user))
                return true;

            var roleIds = await GetCurrentUserRoleIds(user);

            return await _context.RoleMenus
                .AnyAsync(x =>
                    roleIds.Contains(x.RoleId) &&
                    x.MenuId == menuId);
        }

        public async Task<bool> CanViewColumnAsync(
            string entityName,
            string columnName,
            string permissionName)
        {
            var user = await GetCurrentUser();

            if (user == null)
                return false;

            if (await IsSystemAdmin(user))
                return true;

            var permission = await _context.PermissionMasters
                .FirstOrDefaultAsync(x => x.Name == permissionName);

            if (permission == null)
                return false;

            // USER OVERRIDE FIRST
            var userColumn = await _context.ColumnPermissions
                .Where(x =>
                    x.UserId == user.Id &&
                    x.PermissionId == permission.Id &&
                    x.EntityName == entityName &&
                    x.ColumnName == columnName)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            if (userColumn != null)
                return userColumn.CanView;

            // ROLE BASED
            var roleIds = await GetCurrentUserRoleIds(user);

            var roleColumn = await _context.ColumnPermissions
                .Where(x =>
                    roleIds.Contains(x.RoleId) &&
                    x.PermissionId == permission.Id &&
                    x.EntityName == entityName &&
                    x.ColumnName == columnName)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            return roleColumn?.CanView ?? true;
        }

        public async Task<bool> CanEditColumnAsync(
            string entityName,
            string columnName,
            string permissionName)
        {
            var user = await GetCurrentUser();

            if (user == null)
                return false;

            if (await IsSystemAdmin(user))
                return true;

            var permission = await _context.PermissionMasters
                .FirstOrDefaultAsync(x => x.Name == permissionName);

            if (permission == null)
                return false;

            var userColumn = await _context.ColumnPermissions
                .Where(x =>
                    x.UserId == user.Id &&
                    x.PermissionId == permission.Id &&
                    x.EntityName == entityName &&
                    x.ColumnName == columnName)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            if (userColumn != null)
                return userColumn.CanEdit;

            var roleIds = await GetCurrentUserRoleIds(user);

            var roleColumn = await _context.ColumnPermissions
                .Where(x =>
                    roleIds.Contains(x.RoleId) &&
                    x.PermissionId == permission.Id &&
                    x.EntityName == entityName &&
                    x.ColumnName == columnName)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            return roleColumn?.CanEdit ?? true;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _userManager.Users
                .FirstOrDefaultAsync(x =>
                    x.UserName == _userContext.UserName);
        }

        private async Task<bool> IsSystemAdmin(ApplicationUser user)
        {
            return
                await _userManager.IsInRoleAsync(user, AppRoles.ADMIN)
                || await _userManager.IsInRoleAsync(user, AppRoles.SUPERADMIN);
        }

        private async Task<List<string>> GetCurrentUserRoleIds(ApplicationUser user)
        {
            var roleNames = await _userManager.GetRolesAsync(user);

            return await _identityContext.Roles
                .Where(x => roleNames.Contains(x.Name))
                .Select(x => x.Id)
                .ToListAsync();
        }
    }
}