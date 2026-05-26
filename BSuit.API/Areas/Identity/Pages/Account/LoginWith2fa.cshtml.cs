// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using BSuit.API.Infrastructure.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace BSuit.API.Areas.Identity.Pages.Account
{
    public class LoginWith2faModel : PageModel
    {
        private readonly SignInManager<BSuit.Identity.Models.ApplicationUser> _signInManager;
        private readonly UserManager<BSuit.Identity.Models.ApplicationUser> _userManager;
        private readonly RoleManager<BSuit.Identity.Models.ApplicationRole> _roleManager;
        private readonly ILogger<LoginWith2faModel> _logger;

        public LoginWith2faModel(
            SignInManager<BSuit.Identity.Models.ApplicationUser> signInManager,
            UserManager<BSuit.Identity.Models.ApplicationUser> userManager,
            ILogger<LoginWith2faModel> logger,
            RoleManager<BSuit.Identity.Models.ApplicationRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _roleManager = roleManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool RememberMe { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Authenticator code")]
            public string TwoFactorCode { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember this machine")]
            public bool RememberMachine { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            ReturnUrl = returnUrl;
            RememberMe = rememberMe;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            var authenticatorCode = Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, Input.RememberMachine);

            var userId = await _userManager.GetUserIdAsync(user);

            if (result.Succeeded)
            {
                // Remove default Identity cookie created by TwoFactorAuthenticatorSignInAsync
                //Default Identity login won't include these claims.
                await _signInManager.SignOutAsync();

                var roles = await _userManager.GetRolesAsync(user);

               
                // Create your custom claims cookie
                await SignInWithClaimsAsync(user, roles);

                _logger.LogInformation("User with ID '{UserId}' logged in with 2FA.", user.Id);

                return LocalRedirect(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);
                return RedirectToPage("./Lockout");
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return Page();
            }
        }


        private async Task SignInWithClaimsAsync(
                  BSuit.Identity.Models.ApplicationUser user,
                  IList<string> roles)
        {
            var claims = new List<Claim>
    {
        new Claim(
            ClaimTypes.NameIdentifier,
            user.Id.ToString()),

        new Claim(
            ClaimTypes.Email,
            user.Email ?? string.Empty),

        new Claim(
            ClaimTypes.Name,
            user.UserName ?? string.Empty)
    };

            //------------------------------------------
            // Fetch all role entities in one DB call
            //------------------------------------------
            var roleEntities = await _roleManager.Roles
                .Where(x => roles.Contains(x.Name))
                .ToListAsync();

            foreach (var role in roleEntities)
            {
                // Role Name Claim
                claims.Add(new Claim(
                    ClaimTypes.Role,
                    role.Name));

                // Role Id Claim
                claims.Add(new Claim(
                    "RoleId",
                    role.Id.ToString()));
            }

            //------------------------------------------
            // Tenant Claim
            //------------------------------------------
            if (user.TenantId.HasValue)
            {
                claims.Add(new Claim(
                    PARAMS.TENANT_ID,
                    user.TenantId.Value.ToString()));
            }

            //------------------------------------------
            // Super Admin Claim
            //------------------------------------------
            if (roles.Contains(nameof(BSuit.Identity.AppRoles.SUPERADMIN)))
            {
                claims.Add(new Claim(
                    PARAMS.ADMIN,
                    "true"));
            }

            //------------------------------------------
            // Custom User Guid Claim
            //------------------------------------------
            if (user.UserId.HasValue)
            {
                claims.Add(new Claim(
                    PARAMS.GUID_USER_ID,
                    user.UserId.Value.ToString()));
            }

            //------------------------------------------
            // Sign In
            //------------------------------------------
            var identity = new ClaimsIdentity(
                claims,
                IdentityConstants.ApplicationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                IdentityConstants.ApplicationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = RememberMe
                });
        }

    }
}
