#nullable disable
using BSuit.API.Infrastructure.Constants;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace BSuit.API.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {

        #region Constructor and Parameters      
        private readonly SignInManager<BSuit.Identity.Models.ApplicationUser> _signInManager;
        private readonly UserManager<BSuit.Identity.Models.ApplicationUser> _userManager;
        private readonly RoleManager<BSuit.Identity.Models.ApplicationRole> _roleManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(
            SignInManager<BSuit.Identity.Models.ApplicationUser> signInManager,
            UserManager<BSuit.Identity.Models.ApplicationUser> userManager,
             RoleManager<BSuit.Identity.Models.ApplicationRole> roleManager,
            ILogger<LoginModel> logger)
        {
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }


        [BindProperty]
        public string CaptchaInput { get; set; }



        #endregion



        // GET     
        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(ErrorMessage))
                {
                    ModelState.AddModelError(string.Empty, ErrorMessage);
                }

                returnUrl ??= Url.Content("~/");

                if (!Url.IsLocalUrl(returnUrl))
                {
                    _logger.LogWarning("Invalid return URL detected.");
                    returnUrl = Url.Content("~/");
                }

                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

                ReturnUrl = returnUrl;

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnGetAsync Login.");
                ErrorMessage = "Unexpected error occurred. Please try again.";
                return Page();
            }
        }


        // POST        
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            try
            {
                var sessionCaptcha = HttpContext.Session.GetString("CaptchaCode");

                if (string.IsNullOrWhiteSpace(CaptchaInput) ||
                    string.IsNullOrWhiteSpace(sessionCaptcha) ||
                    !string.Equals(
                        CaptchaInput.Trim(),
                        sessionCaptcha.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(string.Empty, "Invalid captcha.");
                    return Page();
                }
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Captcha expired.");
                return Page();
            }


            returnUrl = GetSafeReturnUrl(returnUrl);

            if (!ModelState.IsValid)
                return Page();

            try
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);

                if (user == null)
                {
                    return InvalidLogin();
                }

                if (_userManager.Options.SignIn.RequireConfirmedAccount &&
                    !await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed.");
                    return Page();
                }

                // Password
                // Validate password first
                if (!await _userManager.CheckPasswordAsync(user, Input.Password))
                {
                    return InvalidLogin();
                }

                // Check if 2FA is enabled
                if (await _userManager.GetTwoFactorEnabledAsync(user))
                {
                    await HttpContext.SignInAsync(IdentityConstants.TwoFactorUserIdScheme,
                        StoreTwoFactorInfo(user.Id, Input.RememberMe));

                    _logger.LogInformation("2FA required for user: {Email}", user.Email);

                    // store rememberMe + returnUrl
                    return RedirectToPage(
                        "./LoginWith2fa",
                        new
                        {
                            ReturnUrl = returnUrl,
                            RememberMe = Input.RememberMe
                        });
                }

                // Normal login if 2FA disabled
                var roles = await _userManager.GetRolesAsync(user);

                await SignInWithClaimsAsync(user, roles);

                _logger.LogInformation("User logged in successfully: {Email}", Input.Email);

                return RedirectToDashboard(returnUrl, roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for user: {Email}", Input?.Email);
                ModelState.AddModelError(string.Empty, "Something went wrong. Please try again.");
                return Page();
            }
        }



        private string GetSafeReturnUrl(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
                return Url.Content("~/");

            if (!Url.IsLocalUrl(returnUrl))
            {
                _logger.LogWarning("Invalid return URL.");
                return Url.Content("~/");
            }

            return returnUrl;
        }

        private IActionResult InvalidLogin()
        {
            _logger.LogWarning("Invalid login attempt for: {Email}", Input.Email);
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
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
                    IsPersistent = false
                });
        }

        //private IActionResult RedirectToDashboard(string returnUrl = "/", IList<string> roles = null)
        //{
        //    if (roles == null)
        //    {
        //        return LocalRedirect("/Identity/Account/Login");
        //    }

        //    if (roles.Contains(nameof(BSuit.Identity.AppRoles.SUPERADMIN)) ||
        //                roles.Contains(nameof(BSuit.Identity.AppRoles.ADMIN)))
        //    {
        //        return LocalRedirect("/Admin/Dashboard");
        //    }

        //    if (roles.Contains(nameof(BSuit.Identity.AppRoles.TENANT)))
        //    {
        //        return LocalRedirect("/Admin/Dashboard");//Tenant
        //    }

        //    return LocalRedirect("/SalesCRM/Lead/Dashboard");
        //    //return LocalRedirect("/BSuit/SalesCRM/Lead/Dashboard");

        //    //return LocalRedirect(returnUrl);
        //}

        private IActionResult RedirectToDashboard(string returnUrl = "/", IList<string> roles = null)
        {
            string baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            if (roles == null)
            {
                return Redirect($"{baseUrl}/Identity/Account/Login");
            }

            if (roles.Contains(nameof(BSuit.Identity.AppRoles.SUPERADMIN)) ||
                roles.Contains(nameof(BSuit.Identity.AppRoles.ADMIN)))
            {
                return Redirect($"{baseUrl}/Admin/Dashboard");
            }

            if (roles.Contains(nameof(BSuit.Identity.AppRoles.TENANT)))
            {
                return Redirect($"{baseUrl}/Admin/Dashboard");
            }
            if(roles.Contains("Sales Manager"))
            {
                return LocalRedirect("/SalesCRM/Lead/Dashboard");//Tenant
            }
            return Redirect($"{baseUrl}/SalesCRM/Lead/Dashboard");
        }


        private ClaimsPrincipal StoreTwoFactorInfo(string userId, bool rememberMe)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userId),
                new Claim("RememberMe", rememberMe.ToString())
            };

            var identity = new ClaimsIdentity(
                claims,
                IdentityConstants.TwoFactorUserIdScheme
            );

            return new ClaimsPrincipal(identity);
        }



        private string GenerateCaptchaCode()
        {
            //Random
            //const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            //var random = new Random();

            //return new string(
            //    Enumerable.Repeat(chars, 6)
            //        .Select(x => x[random.Next(x.Length)])
            //        .ToArray());


            //Dictionary based
            var random = new Random();

            var word = BSuit.API.Infrastructure.CaptchaWordGenerator.Generate();
            return word;

            //var digits = random.Next(10, 99);
            //return $"{word}{digits}";
        }

      

        public IActionResult OnGetCaptcha()
        {
            string captchaCode = GenerateCaptchaCode();

            HttpContext.Session.SetString("CaptchaCode", captchaCode);

            using var image = new Image<Rgba32>(180, 60);

            image.Mutate(ctx =>
            {
                ctx.Fill(Color.White);

                var random = new Random();

                // Background lines
                for (int i = 0; i < 8; i++)
                {
                    ctx.DrawLine(
                        Color.LightGray,
                        1,
                        new PointF(random.Next(180), random.Next(60)),
                        new PointF(random.Next(180), random.Next(60))
                    );
                }

                // Noise dots
                for (int i = 0; i < 100; i++)
                {
                    ctx.Fill(
                        Color.Gray,
                        new Rectangle(
                            random.Next(180),
                            random.Next(60),
                            2,
                            2
                        )
                    );
                }

                var font = SystemFonts.CreateFont("Arial", 30);

                for (int i = 0; i < captchaCode.Length; i++)
                {
                    ctx.DrawText(
                        captchaCode[i].ToString(),
                        font,
                        Color.Black,
                        new PointF(20 + (i * 25), random.Next(5, 20))
                    );
                }
            });

            using var ms = new MemoryStream();
            image.SaveAsPng(ms);
            ms.Position = 0;

            return File(ms.ToArray(), "image/png");
        }
    }
}