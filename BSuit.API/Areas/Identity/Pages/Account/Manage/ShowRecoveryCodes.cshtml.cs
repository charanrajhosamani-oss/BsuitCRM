using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BSuit.Identity.Models;

namespace BSuit.API.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class ShowRecoveryCodesModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ShowRecoveryCodesModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public string[] RecoveryCodes { get; set; }

        public IActionResult OnGet()
        {
            var recoveryCodes = TempData["RecoveryCodes"] as string[];

            if (recoveryCodes == null || recoveryCodes.Length == 0)
            {
                return RedirectToPage("./TwoFactorAuthentication");
            }

            RecoveryCodes = recoveryCodes;

            return Page();
        }
    }
}