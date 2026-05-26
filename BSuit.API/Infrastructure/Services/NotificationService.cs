using BSuit.Core.Data;
using BSuit.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Models;

namespace BSuit.API.Infrastructure.Services
{
    public interface INotificationService
    {
        Task CreateForUsers(
            string title,
            string message,
            List<string> userIds,
            string createdBy);
      
        Task<int> GetUnreadCount(string userId);

        Task<List<NotificationRecipient>> GetUnreadNotifications(string userId);

        Task MarkAsRead(int recipientId);

        Task MarkAllAsRead(string userId);
    }

    public class NotificationService : INotificationService
    {
        private readonly CoreDbContext _context;

        public NotificationService(CoreDbContext context)
        {
            _context = context;
        }

        public async Task CreateForUsers(
            string title,
            string message,
            List<string> userIds,
            string createdBy)
        {
            var notification = new NotificationMaster
            {
                Title = title,
                Message = message,
                CreatedByUserId = createdBy
            };

            _context.NotificationMasters.Add(notification);
            await _context.SaveChangesAsync();

            foreach (var userId in userIds)
            {
                _context.NotificationRecipients.Add(
                    new NotificationRecipient
                    {
                        NotificationMasterId = notification.Id,
                        UserId = userId
                    });
            }

            await _context.SaveChangesAsync();
        }

      

        public async Task<int> GetUnreadCount(string userId)
        {
            return await _context.NotificationRecipients
                .CountAsync(x =>
                    x.UserId == userId &&
                    !x.IsRead &&
                    !x.IsDeleted);
        }

        public async Task<List<NotificationRecipient>> GetUnreadNotifications(string userId)
        {
            return await _context.NotificationRecipients
                .Include(x => x.Notification)
                .Where(x =>
                    x.UserId == userId &&
                    !x.IsRead)
                .OrderByDescending(x => x.CreatedOn)
                .Take(5)
                .ToListAsync();
        }

        public async Task MarkAsRead(int recipientId)
        {
            var item = await _context.NotificationRecipients
                .FirstOrDefaultAsync(x => x.Id == recipientId);

            if (item != null)
            {
                item.IsRead = true;
                item.ReadOn = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsRead(string userId)
        {
            var notifications = await _context.NotificationRecipients
                .Where(x => x.UserId == userId && !x.IsRead)
                .ToListAsync();

            foreach (var item in notifications)
            {
                item.IsRead = true;
                item.ReadOn = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }
}


/*
 [HttpPost]
public async Task<IActionResult> Create(NotificationVM vm)
{
    var currentUser = _userManager.GetUserId(User);

    if (vm.UserIds.Any())
    {
        await _notificationService.CreateForUsers(
            vm.Title,
            vm.Message,
            vm.UserIds,
            currentUser);
    }

    if (vm.TeamIds.Any())
    {
        await _notificationService.CreateForTeams(
            vm.Title,
            vm.Message,
            vm.TeamIds,
            currentUser);
    }

    return RedirectToAction("Index");
}
*/