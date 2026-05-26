using BSuit.API.Areas.Admin.Models;
using BSuit.API.Areas.Admin.Services;
using BSuit.Contracts.Services;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace BSuit.API.Areas.Admin.Controllers
{
    public class DashboardController : Base.BaseAdminController
    {
        private readonly IUserContext _userContext;
        private readonly IDashboardService _dashboardService;

        public DashboardController(
            IUserContext userContext,
            IDashboardService dashboardService)
        {
            _userContext = userContext;
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            if (!_userContext.IsAuthenticated)
                return Unauthorized();

            var vm = await _dashboardService.GetDashboardAsync(User);

            ViewBag.ChartLabels = JsonSerializer.Serialize(vm.WeeklyUsers.Select(x => x.Label));
            ViewBag.ChartData = JsonSerializer.Serialize(vm.WeeklyUsers.Select(x => x.Value));

            return View(vm);
        }
    }
}