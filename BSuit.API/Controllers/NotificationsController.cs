using BSuit.API.Infrastructure.Services;
using BSuit.Core.Data;
using BSuit.Core.Entities;
using BSuit.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BSuit.API.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CoreDbContext _context;

        public NotificationsController(
            INotificationService notificationService,
            UserManager<ApplicationUser> userManager,
            CoreDbContext context)
        {
            _notificationService = notificationService;
            _userManager = userManager;
            _context = context;
        }

     

        // Mark all unread notifications as read
        public async Task<IActionResult> MarkAllRead()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            await _notificationService.MarkAllAsRead(
                user.Id
            );         
          
            return Redirect(Request.Headers["Referer"].ToString());
        }

        // Open specific notification
        public async Task<IActionResult> Read(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            await _notificationService.MarkAsRead(id);

            return RedirectToAction(nameof(Details), new { id });
        }

        // Full notification details page
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var notification = await _context.NotificationRecipients
                .Include(x => x.Notification)
                //.ThenInclude(n => n.CreatedByUser)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.UserId == user.Id);

            if (notification == null)
                return NotFound();

            return View(notification);
        }

        // View all notifications
        public async Task<IActionResult> ViewAll(int page = 1)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            int pageSize = 10;

            var query = _context.NotificationRecipients
                .Include(x => x.Notification)
                .Where(x => x.UserId == user.Id)
                .OrderByDescending(x => x.Notification.CreatedOn);

            var totalRecords = await query.CountAsync();

            var notifications = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalRecords = totalRecords;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            return View(notifications);
        }
    }
}