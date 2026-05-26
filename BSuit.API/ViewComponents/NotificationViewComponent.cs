using BSuit.API.Infrastructure.Services;
using BSuit.Core.Entities;
using BSuit.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BSuit.API.ViewComponents
{
    public class NotificationHeaderVM
    {
        public int UnreadCount { get; set; }

        public List<NotificationRecipient> Notifications { get; set; }
            = new List<NotificationRecipient>();
    }

    public class NotificationViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;

        public NotificationViewComponent(
            UserManager<ApplicationUser> userManager,
            INotificationService notificationService)
        {
            _userManager = userManager;
            _notificationService = notificationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user == null)
                return View(new NotificationHeaderVM());

            var unreadCount = await _notificationService
                .GetUnreadCount(user.Id);

            var notifications = await _notificationService
                .GetUnreadNotifications(user.Id);

            var vm = new NotificationHeaderVM
            {
                UnreadCount = unreadCount,
                Notifications = notifications
            };

            return View(vm);
        }


    }
}
