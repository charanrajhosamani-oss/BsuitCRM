// Areas/Admin/Controllers/AdminUsersController.cs
using BSuit.API.Areas.Admin.Controllers.Base;
using BSuit.API.Areas.Admin.Models;
using BSuit.API.Infrastructure.Constants;
using BSuit.Identity;
using BSuit.Identity.Data;
using BSuit.Identity.Models;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace BSuit.API.Areas.Admin.Controllers
{
    // Entire controller accessible only to SUPERADMIN
    [Authorize(Policy = POLICIES.SuperadminOnly)]
    public class AdminUsersController : BaseAdminController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IdentityDbContext _context;

        public AdminUsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IdentityDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index(AdminUserVM model)
        {
            var query = _userManager.Users.AsQueryable();

            // Get ADMIN + SUPERADMIN role ids from AspNetRoles using RoleManager
            var roleIds = await _roleManager.Roles
                .Where(x =>
                    x.Name == AppRoles.ADMIN ||
                    x.Name == AppRoles.SUPERADMIN)
                .Select(x => x.Id)
                .ToListAsync();

            // Get user ids having those roles from AspNetUserRoles
            var userIds = await _context.UserRoles
                .Where(x => roleIds.Contains(x.RoleId))
                .Select(x => x.UserId)
                .Distinct()
                .ToListAsync();

            // Show only ADMIN + SUPERADMIN users
            query = query.Where(x => userIds.Contains(x.Id));

            // SEARCH
            if (!string.IsNullOrEmpty(model.Search))
            {
                query = query.Where(x =>
                    x.Email.Contains(model.Search) ||
                    x.UserName.Contains(model.Search));
            }

            // SORTING
            query = model.SortColumn switch
            {
                "Email" => model.SortDirection == PARAMS.ASC
                    ? query.OrderBy(x => x.Email)
                    : query.OrderByDescending(x => x.Email),

                _ => query.OrderBy(x => x.UserName)
            };

            // TOTAL COUNT
            model.TotalCount = await query.CountAsync();

            // PAGINATION
            model.Users = await query
                .Skip((model.PageNumber - 1) * model.PageSize)
                .Take(model.PageSize)
                .ToListAsync();
            
            
            foreach (var item in model.Users)
            {
                var roles = await _userManager.GetRolesAsync(item);

                if (roles.Contains(AppRoles.SUPERADMIN, StringComparer.OrdinalIgnoreCase))
                {
                    item.SelectedRole = AppRoles.SUPERADMIN;
                    item.IsSuperAdmin = true;
                }
                else if (roles.Contains(AppRoles.ADMIN, StringComparer.OrdinalIgnoreCase))
                {
                    item.SelectedRole = AppRoles.ADMIN;
                    item.IsSuperAdmin = false;
                }
                else
                {
                    item.SelectedRole = roles.FirstOrDefault()?.ToUpper();
                    item.IsSuperAdmin = false;
                }
            }

            // Dropdown Roles from AspNetRoles using RoleManager
            model.Roles = await _roleManager.Roles
                .Where(x =>
                    x.Name == AppRoles.ADMIN ||
                    x.Name == AppRoles.SUPERADMIN)
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Name
                })
                .ToListAsync();

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> Create(string email, string password, string roleName)
        {
            var user = new ApplicationUser
            {
                Email = email,
                UserName = email,
                IsActive = true,
                EmailConfirmed = true,
            };

            // This automatically hashes and stores password in AspNetUsers table
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return RedirectToAction(nameof(Index));
            }

            //Prevent Circular Reporting Structure
            if (user.Id == user.ReportingManagerId)
            {
                ModelState.AddModelError("", "User cannot report to themselves.");
            }

            // Optional: assign role after creation
            await _userManager.AddToRoleAsync(user, roleName);
            TempData["Success"] = "User created successfully";
            return RedirectToAction(nameof(Index));
        }

        // ================= EDIT =================
        [HttpPost]
        public async Task<IActionResult> Edit(string id, string email, string password)
        {
            var user = await _userManager.FindByIdAsync(id);
            user.Email = email;
            user.UserName = email;

            await _userManager.UpdateAsync(user);
            TempData["Success"] = "User updated successfully.";

            // Update Password only if new password is entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                // Identity requires reset token for password change
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var passwordResult = await _userManager.ResetPasswordAsync(
                    user,
                    token,
                    password);

                if (!passwordResult.Succeeded)
                {
                    foreach (var error in passwordResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    TempData["Error"] = "Failed to updated Password.";

                }
                else
                {
                    TempData["Success"] = "Password created successfully.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // ================= TOGGLE ACTIVE =================
        [HttpPost]
        public async Task<IActionResult> ToggleActive(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            user.IsActive = !user.IsActive;

            await _userManager.UpdateAsync(user);
            TempData["Success"] = "Status changed successfully";
            return RedirectToAction(nameof(Index));
        }

        // ================= DELETE =================
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(user);
            TempData["Success"] = "Profile removed successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}