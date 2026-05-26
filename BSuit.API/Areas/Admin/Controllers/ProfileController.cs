// Areas/Admin/Controllers/ProfileController.cs
using BSuit.API.Areas.Admin.Controllers.Base;
using BSuit.API.Areas.Admin.Models;
using BSuit.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BSuit.API.Areas.Admin.Controllers
{
    public class ProfileController : BaseAdminController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ================= MY PROFILE =================      
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                TempData["Error"] = "User not found";
                return RedirectToAction("Login", "Account");
            }

            var model = new ProfileVM
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Active = user.IsActive,
                IsTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
                ChangePasswordVM =new ChangePasswordVM()
                {
                     UserName = user.UserName
                }
            };

            ModelState.Clear();   // Fix
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileVM model)
        {
            if(string.IsNullOrEmpty(model.Id) == true)
            {
                TempData["Error"] = "Failed to update profile.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(model.Id);

            user.Email = model.Email;
            user.UserName = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            await _userManager.UpdateAsync(user);
            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ================= CHANGE PASSWORD =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (string.IsNullOrEmpty(model.NewPassword) == true)
            {
                TempData["Error"] = "New Password is required.";
                return RedirectToAction(nameof(Index));
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                TempData["Error"] = "New and Confirm password mismatch";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.GetUserAsync(User);

            var result = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword);

            if (result.Succeeded)
            {
                TempData["Success"] = "Password changed successfully.";
                await _signInManager.RefreshSignInAsync(user);
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            TempData["Error"] = "Failed to change Password.";
            return View("Index");
        }
    }
}