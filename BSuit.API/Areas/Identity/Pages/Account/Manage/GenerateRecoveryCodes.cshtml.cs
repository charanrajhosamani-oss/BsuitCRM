using BSuit.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BSuit.API.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class GenerateRecoveryCodesModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GenerateRecoveryCodesModel(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user.");
            }

            var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);

            if (!isTwoFactorEnabled)
            {
                throw new InvalidOperationException(
                    $"Cannot generate recovery codes for user because they do not have 2FA enabled.");
            }

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

            TempData["RecoveryCodes"] = recoveryCodes.ToArray();

            return RedirectToPage("./ShowRecoveryCodes");
        }
    }
}