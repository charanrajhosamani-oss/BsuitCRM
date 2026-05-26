using BSuit.API.Areas.Admin.Models;
using BSuit.API.Infrastructure;
using BSuit.API.Models;
using BSuit.Contracts.Services;
using BSuit.Core.Data;
using BSuit.Core.Entities;
using BSuit.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BSuit.API.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly CoreDbContext _context;
        private readonly IUserContext _userContext;


        public HomeController(
            ILogger<HomeController> logger,
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



        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> MyProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            var tenant = await _context.Tenants.FirstOrDefaultAsync(x => x.Id == user.TenantId);

            var roles = await _userManager.GetRolesAsync(user);

            var vm = new MyProfileVM
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RoleName = roles.FirstOrDefault(),
                IsActive = user.IsActive,


                TenantId = tenant.Id,
                TenantName = tenant.Name,
                TenantEmail = tenant.Email,
                DomainName = tenant.Domain,
                CreatedOn = tenant.CreatedOn,
                TenantStatus = tenant.IsActive == true ? "Active" : "Inactive",
            };

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(MyProfileVM vm)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Json(new { success = false });

            user.FullName = vm.FullName;
            user.PhoneNumber = vm.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            return Json(new
            {
                success = result.Succeeded,
                message = result.Succeeded ? "Profile updated" : "Update failed"
            });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(MyProfileVM vm)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Json(new { success = false });

            var result = await _userManager.ChangePasswordAsync(
                user,
                vm.CurrentPassword,
                vm.NewPassword
            );

            if (!result.Succeeded)
            {
                return Json(new
                {
                    success = false,
                    message = string.Join(",", result.Errors.Select(x => x.Description))
                });
            }

            return Json(new
            {
                success = true,
                message = "Password updated successfully"
            });
        }


        [HttpPost]
        public async Task<IActionResult> LogoutOnTabClose()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }


        //3. Store in Session
        //4. Enable Session (program.cs)
        ///builder.Services.AddSession();
        ///app.UseSession();
        [HttpPost]
        public IActionResult SetTimeZone([FromBody] TimeZoneRequest model)
        {
            HttpContext.Session.SetString(
                "UserTimeZone",
                model.TimeZone);

            return Ok();
        }

        //7. Usage in Controller
        //var localTime = DateTimeHelper.ToUserTime(entity.CreatedOn, HttpContext);

        //8. Razor Usage
        //@inject IHttpContextAccessor HttpContextAccessor
        //@using BSuit.API.Infrastructure
        //@DateTimeHelper.ToUserTime(item.CreatedOn, HttpContextAccessor.HttpContext).ToString("dd-MMM-yyyy hh:mm tt")
    }
}
