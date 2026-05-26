// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace BSuit.API.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<BSuit.Identity.Models.ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<BSuit.Identity.Models.ApplicationUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        
        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            string baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return Redirect($"{baseUrl}{returnUrl}");
            }
            else
            {
                // Always go to login page
               // return RedirectToPage("{baseUrl}//Account/Login", new { area = "Identity" });
                return RedirectToPage($"{baseUrl}/Identity/Account/Login");
            }

            
        }
    }
}
