using BSuit.Identity.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSuit.Core.Entities
{
    public class NotificationMaster : _BASE
    {
        public Guid TenantId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationType NotificationType { get; set; }

        public string CreatedByUserId { get; set; }      
        
        public ICollection<NotificationRecipient> Recipients { get; set; }

    }

    public enum NotificationType
    {
        None,
        Success,
        Error,
        Warning,
        Info
    }
}
