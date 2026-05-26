using BSuit.Identity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSuit.Core.Entities
{
    public class NotificationRecipient : _BASE
    {
        public int NotificationMasterId { get; set; }
        public NotificationMaster Notification { get; set; }

        public string UserId { get; set; }       

        public bool IsRead { get; set; } = false
    ;
        public DateTime? ReadOn { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
