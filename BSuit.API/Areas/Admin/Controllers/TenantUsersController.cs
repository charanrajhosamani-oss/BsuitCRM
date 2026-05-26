using BSuit.API.Areas.Admin.Controllers.Base;
using BSuit.API.Areas.Admin.Models;
using BSuit.Core.Data;
using BSuit.Identity;
using BSuit.Identity.Models;
using BSuit.SalesCRM.Data;
using BSuit.SalesCRM.Entities;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BSuit.API.Areas.Admin.Controllers
{
    public class TenantUsersController : BaseAdminController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly CoreDbContext _context;
        private readonly SalesCRMContext _salesContext;
        public TenantUsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            CoreDbContext context,
             SalesCRMContext salesContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _salesContext = salesContext;
        }

        public async Task<IActionResult> Index(Guid tenantId, string? search = null)
        {
            var tenant = await _context.Tenants
                .FirstOrDefaultAsync(x => x.Id == tenantId);

            if (tenant == null)
                return NotFound();

            var users = await _userManager.Users
                .Where(x => x.TenantId == tenantId)
                .ToListAsync();

            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(u => u.FullName.Contains(search) ||
                                            u.Email.Contains(search)).ToList();
            }

            var allRoles = await _roleManager.Roles
                                 .Select(x => new
                                 {
                                     x.Id,
                                     x.Name
                                 })
                                 .ToListAsync();

            foreach (var user in users)
            {
                var userRoleNames = await _userManager.GetRolesAsync(user);

                var matchedRoles = allRoles
                    .Where(r => userRoleNames.Contains(r.Name))
                    .ToList();

                user.SelectedRoles = matchedRoles
                    .Select(x => x.Id)
                    .ToList();   // for edit modal

                user.SelectedRole = string.Join(", ",
                    matchedRoles.Select(x => x.Name)); // display

                // Check if user is SuperAdmin
                user.IsSuperAdmin = userRoleNames.Any(x =>
                    x.Equals(nameof(AppRoles.SUPERADMIN),
                    StringComparison.OrdinalIgnoreCase) || x.Equals(nameof(AppRoles.ADMIN),
                    StringComparison.OrdinalIgnoreCase));
            }

            users = users
                .Where(x => x.IsSuperAdmin == false)
                .ToList();


            var currentUser = await _userManager.GetUserAsync(User);

            bool isAdmin = await _userManager.IsInRoleAsync(
                currentUser,
                nameof(AppRoles.ADMIN));

            bool isSuperAdmin = await _userManager.IsInRoleAsync(
                currentUser,
                nameof(AppRoles.SUPERADMIN));

            var rolesQuery = _roleManager.Roles.AsQueryable();

            if (!isAdmin && !isSuperAdmin)
            {
                rolesQuery = rolesQuery.Where(x =>
                    (x.TenantId == tenantId || x.IsSystemRole));
            }

            var roles = await rolesQuery
                .OrderBy(x => x.Name)
                .Select(x => new RoleDropdownVM
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();

            var vm = new TenantUsersVM
            {
                TenantId = tenantId,
                TenantName = tenant.Name,
                Users = users,

                ReportingManagers = await _userManager.Users
                    .Where(x => x.TenantId == tenantId)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id,
                        Text = x.FullName
                    })
                    .ToListAsync(),

                Supervisors = await _userManager.Users
                    .Where(x => x.TenantId == tenantId)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id,
                        Text = x.FullName
                    })
                    .ToListAsync()
            };

            vm.RoleList = roles
                        .Select(x => new SelectListItem
                        {
                            Value = x.Id,
                            Text = x.Name
                        })
                        .ToList();

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            Guid tenantId,
            string fullName,
            string email,
            string password,
            List<string> SelectedRoleIds,
            string? reportingManagerId,
            string? supervisorId)
        {
            var existing = await _userManager.FindByEmailAsync(email);

            if (existing != null)
            {
                TempData["Error"] = "User already exists";
                return RedirectToAction(nameof(Index), new { userId = tenantId });
            }



            var user = new ApplicationUser
            {
                FullName = fullName,
                UserName = email,
                Email = email,
                TenantId = tenantId,
                ReportingManagerId = reportingManagerId,
                SupervisorId = supervisorId,
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                TempData["Error"] = string.Join(",",
                    result.Errors.Select(x => x.Description));

                return RedirectToAction(nameof(Index), new { userId = tenantId });
            }

            //await _userManager.AddToRoleAsync(user, role);

            var roleNames = await _roleManager.Roles
                       .Where(x => SelectedRoleIds.Contains(x.Id))
                       .Select(x => x.Name)
                       .ToListAsync();
            await _userManager.AddToRolesAsync(user, roleNames);

            TempData["Success"] = "User created successfully";

            return RedirectToAction(nameof(Index), new { userId = tenantId });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActive(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            user.IsActive = !user.IsActive;

            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index),
                new { tenantId = user.TenantId });
        }


        [HttpPost]
        public async Task<IActionResult> EditAjax(EditUserVM vm)
        {
            var user = await _userManager.FindByIdAsync(vm.Id);

            if (user == null)
            {
                return Json(new
                {
                    success = false,
                    message = "User not found"
                });
            }

            using var transaction =
                await _salesContext.Database.BeginTransactionAsync();

            try
            {
                //--------------------------------------
                // Update basic user details
                //--------------------------------------
                user.FullName = vm.FullName;
                user.Email = vm.Email;
                user.UserName = vm.Email;
                user.ReportingManagerId = vm.ReportingManagerId;
                user.SupervisorId = vm.SupervisorId;

                var updateResult = await _userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    return Json(new
                    {
                        success = false,
                        message = string.Join(", ",
                            updateResult.Errors.Select(x => x.Description))
                    });
                }

                //--------------------------------------
                // Existing roles assigned to user
                //--------------------------------------
                var existingRoleNames = await _userManager.GetRolesAsync(user);

                // Fetch existing role entities for current tenant/system roles
                var existingRoleEntities = await _roleManager.Roles
                    .Where(x =>
                        existingRoleNames.Contains(x.Name) &&
                        (
                            x.TenantId == user.TenantId ||
                            x.IsSystemRole
                        ))
                    .ToListAsync();


                //--------------------------------------
                // Newly selected roles
                //--------------------------------------
                var selectedRoleEntities = await _roleManager.Roles
                    .Where(x =>
                        vm.SelectedRoleIds.Contains(x.Id) &&
                        (
                            x.TenantId == user.TenantId ||
                            x.IsSystemRole
                        ))
                    .ToListAsync();


                //--------------------------------------
                // Roles to remove
                //--------------------------------------
                var rolesToRemove = existingRoleEntities
                    .Where(x => !vm.SelectedRoleIds.Contains(x.Id))
                    .ToList();


                //--------------------------------------
                // Roles to add
                //--------------------------------------
                var rolesToAdd = selectedRoleEntities
                    .Where(x => !existingRoleNames.Contains(x.Name))
                    .ToList();

                //--------------------------------------
                // Delete service mappings for removed roles
                //--------------------------------------
                if (rolesToRemove.Any())
                {
                    var roleIdsToRemove = rolesToRemove
                        .Select(x => Guid.Parse(x.Id))
                        .ToList();

                    var mappedServices = await _salesContext.ServiceUserRoleMaps
                        .Where(x =>
                            x.UserId == Guid.Parse(user.Id) &&
                            roleIdsToRemove.Contains(x.RoleId))
                        .ToListAsync();

                    if (mappedServices.Any())
                    {
                        _salesContext.ServiceUserRoleMaps.RemoveRange(mappedServices);
                        await _salesContext.SaveChangesAsync();
                    }
                }

                //--------------------------------------
                // Remove roles
                //--------------------------------------
                if (rolesToRemove.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(
                        user,
                        rolesToRemove.Select(x => x.NormalizedName)
                    );

                    if (!removeResult.Succeeded)
                    {
                        await transaction.RollbackAsync();

                        return Json(new
                        {
                            success = false,
                            message = string.Join(", ",
                                removeResult.Errors.Select(x => x.Description))
                        });
                    }
                }

                //--------------------------------------
                // Add new roles
                //--------------------------------------
                if (rolesToAdd.Any())
                {
                    var addResult = await _userManager.AddToRolesAsync(
                        user,
                        rolesToAdd.Select(x => x.NormalizedName)
                    );

                    if (!addResult.Succeeded)
                    {
                        await transaction.RollbackAsync();

                        return Json(new
                        {
                            success = false,
                            message = string.Join(", ",
                                addResult.Errors.Select(x => x.Description))
                        });
                    }
                }

                //--------------------------------------
                // Password update
                //--------------------------------------
                if (!string.IsNullOrWhiteSpace(vm.Password))
                {
                    var token =
                        await _userManager.GeneratePasswordResetTokenAsync(user);

                    var passwordResult =
                        await _userManager.ResetPasswordAsync(
                            user,
                            token,
                            vm.Password);

                    if (!passwordResult.Succeeded)
                    {
                        await transaction.RollbackAsync();

                        return Json(new
                        {
                            success = false,
                            message = string.Join(", ",
                                passwordResult.Errors.Select(x => x.Description))
                        });
                    }
                }

                //--------------------------------------
                // Commit transaction
                //--------------------------------------
                await transaction.CommitAsync();

                return Json(new
                {
                    success = true,
                    message = "User updated successfully"
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }



        //MAPSERVICES
        //Tenant → Role → Users (filtered by selected role) → Services


        public async Task<IActionResult> MapServices(string userId)
        {
            var pulledUser = await _userManager.Users
                .Where(x => x.Id == userId)
                .FirstOrDefaultAsync();

            var tenant = await _context.Tenants
                .FirstOrDefaultAsync(x => x.Id == pulledUser.TenantId);

            if (tenant == null)
                return NotFound();

            var roles = await _roleManager.Roles
                .Where(x => x.TenantId == pulledUser.TenantId.GetValueOrDefault() || x.IsSystemRole)
                .OrderBy(x => x.Name)
                .ToListAsync();

            var services = await _salesContext.Services
                .Where(x => x.IsActive == true)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();

            var mappings = await _salesContext.ServiceUserRoleMaps.ToListAsync();

            if (!string.IsNullOrEmpty(userId))
            {
                Guid guidUserId = Guid.Parse(userId);
                mappings = mappings.Where(u => u.UserId == guidUserId).ToList();
            }

            var servicesData = await _salesContext.Services
                .ToListAsync();

            var usersData = await _userManager.Users
                .Where(x => x.TenantId == pulledUser.TenantId.GetValueOrDefault())
                .ToListAsync();

            var rolesData = await _roleManager.Roles
                .ToListAsync();

            var existingMappings = mappings
                .Where(map => usersData.Any(u => u.Id == map.UserId.ToString()))
                .Select(map => new MappedServiceVM
                {
                    UserId = map.UserId,
                    RoleId = map.RoleId,
                    ServiceId = map.ServiceId,

                    UserName = usersData
                        .FirstOrDefault(u => u.Id == map.UserId.ToString())?.FullName,

                    ServiceName = servicesData
                        .FirstOrDefault(s => s.ServiceId == map.ServiceId)?.ServiceName,

                    RoleName = rolesData
                        .FirstOrDefault(r => r.Id == map.RoleId.ToString())?.Name,

                    AssignedOn = map.AssignedOn
                }).OrderBy(u => u.UserName)
                .ToList();

            var vm = new UserServiceMappingVM
            {
                TenantId = pulledUser.TenantId.GetValueOrDefault(),
                TenantName = tenant.Name,

                Roles = roles.Select(x => new SelectListItem
                {
                    Value = x.Id,
                    Text = x.Name
                }).ToList(),

                Services = services.Select(x => new SelectListItem
                {
                    Value = x.ServiceId.ToString(),
                    Text = x.ServiceName
                }).ToList(),

                ExistingMappings = existingMappings
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersByRole(Guid tenantId, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
                return Json(new List<object>());

            var users = await _userManager.Users
                .Where(x => x.TenantId == tenantId)
                .ToListAsync();

            var filteredUsers = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains(role.Name))
                {
                    filteredUsers.Add(new
                    {
                        id = user.Id,
                        name = user.FullName
                    });
                }
            }

            return Json(filteredUsers);
        }


        [HttpPost]
        public async Task<IActionResult> MapServices(UserServiceMappingVM vm)
        {
            if (string.IsNullOrEmpty(vm.SelectedRoleId))
            {
                TempData["Error"] = "Please select role";
                return RedirectToAction(nameof(MapServices),
                    new { tenantId = vm.TenantId });
            }

            if (vm.SelectedUserIds == null || !vm.SelectedUserIds.Any())
            {
                TempData["Error"] = "Please select users";
                return RedirectToAction(nameof(MapServices),
                    new { tenantId = vm.TenantId });
            }

            if (vm.SelectedServiceIds == null || !vm.SelectedServiceIds.Any())
            {
                TempData["Error"] = "Please select services";
                return RedirectToAction(nameof(MapServices),
                    new { tenantId = vm.TenantId });
            }

            foreach (var userId in vm.SelectedUserIds)
            {
                var parsedUserId = Guid.Parse(userId);

                var existingServiceIds = await _salesContext.ServiceUserRoleMaps
                    .Where(x =>
                        x.UserId == parsedUserId &&
                        x.RoleId == Guid.Parse(vm.SelectedRoleId))
                    .Select(x => x.ServiceId)
                    .ToListAsync();

                var newServices = vm.SelectedServiceIds
                    .Where(x => !existingServiceIds.Contains(x))
                    .ToList();

                foreach (var serviceId in newServices)
                {
                    _salesContext.ServiceUserRoleMaps.Add(new ServiceUserRoleMap
                    {
                        UserId = parsedUserId,
                        RoleId = Guid.Parse(vm.SelectedRoleId),
                        ServiceId = serviceId,
                        AssignedOn = DateTime.UtcNow
                    });
                }
            }

            await _salesContext.SaveChangesAsync();

            TempData["Success"] = "Services mapped successfully";

            return RedirectToAction(nameof(MapServices),
                new { userId = vm.SelectedUserIds.FirstOrDefault() });
        }


        [HttpPost]
        public async Task<IActionResult> DeleteMappedService(
                                            Guid userId,
                                            Guid serviceId,
                                            Guid roleId)
        {
            var mapping = await _salesContext.ServiceUserRoleMaps
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.ServiceId == serviceId &&
                    x.RoleId == roleId);

            if (mapping != null)
            {
                _salesContext.ServiceUserRoleMaps.Remove(mapping);
                await _salesContext.SaveChangesAsync();
            }

            var user = await _userManager.Users
                .Where(x => x.Id == userId.ToString())
                .FirstOrDefaultAsync();

            return RedirectToAction(nameof(MapServices),
                new { userId = userId });
        }










        //2FA


        [HttpPost]
        public async Task<IActionResult> DisableTwoFactor(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["Error"] = "Invalid user.";
                    return RedirectToAction("Index");
                }

                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("Index");
                }

                if (!user.TwoFactorEnabled)
                {
                    TempData["Error"] = "2FA is already disabled for this user.";
                    return RedirectToAction("ManageUsers", new { tenantId = user.TenantId });
                }

                // Disable 2FA
                var result = await _userManager.SetTwoFactorEnabledAsync(user, false);

                if (!result.Succeeded)
                {
                    TempData["Error"] = "Failed to disable 2FA.";
                    return RedirectToAction("ManageUsers", new { tenantId = user.TenantId });
                }

                // Optional: remove old recovery codes
                await _userManager.SetTwoFactorEnabledAsync(user, false);
                await _userManager.ResetAuthenticatorKeyAsync(user);

                TempData["Success"] = $"2FA disabled successfully for {user.Email}";
                return RedirectToAction(nameof(Index), new { tenantId = user.TenantId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Something went wrong.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EnableTwoFactor(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["Error"] = "Invalid user.";
                    return RedirectToAction("Index");
                }

                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("Index");
                }

                if (user.TwoFactorEnabled)
                {
                    TempData["Error"] = "2FA is already enabled for this user.";
                    return RedirectToAction("ManageUsers", new { tenantId = user.TenantId });
                }

                // Disable 2FA
                var result = await _userManager.SetTwoFactorEnabledAsync(user, true);

                if (!result.Succeeded)
                {
                    TempData["Error"] = "Failed to enable 2FA.";
                    return RedirectToAction("ManageUsers", new { tenantId = user.TenantId });
                }

                //// Optional: remove old recovery codes
                //await _userManager.SetTwoFactorEnabledAsync(user, false);
                //await _userManager.ResetAuthenticatorKeyAsync(user);

                TempData["Success"] = $"2FA enabled successfully for {user.Email}";
                return RedirectToAction(nameof(Index), new { tenantId = user.TenantId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Something went wrong.";
                return RedirectToAction("Index");
            }
        }

    }
}