using Microsoft.AspNetCore.Identity;

namespace BSuit.Identity.Models
{
    public class ApplicationRole : IdentityRole
    {
        public Guid TenantId { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }

        public bool IsSystemRole { get; set; }   // true: created by superadmin/admin
        public string CreatedByUserId { get; set; } // optional
    }
}