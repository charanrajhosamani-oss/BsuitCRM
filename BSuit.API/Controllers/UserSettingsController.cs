using BSuit.API.Models;
using BSuit.Contracts.Services;
using BSuit.Core.Data;
using BSuit.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BSuit.API.Controllers
{
    public class UserSettingsController : Controller
    {

        private readonly ILogger<UserSettingsController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly CoreDbContext _context;
        private readonly IUserContext _userContext;


        public UserSettingsController(
            ILogger<UserSettingsController> logger,
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          CoreDbContext context, IUserContext userContext)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _userContext = userContext;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Redirect("/Identity/Account/Login");

            var vm = new UserSettingsVM
            {
                IsTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user)
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> DeactivateProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Json(new { success = false });

            user.IsActive = false;

            await _userManager.UpdateAsync(user);
            await _signInManager.SignOutAsync();

            return Json(new
            {
                success = true,
                redirectUrl = Url.Action("Login", "Account")
            });
        }

    }
}
