using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSuit.Identity.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public Guid? TenantId { get; set; }

        public Guid? UserId { get; set; } = Guid.NewGuid();
        // Audit
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        




        // Reporting hierarchy
        public string? ReportingManagerId { get; set; }
        public ApplicationUser? ReportingManager { get; set; }

        public string? SupervisorId { get; set; }
        public ApplicationUser? Supervisor { get; set; }

        // Reverse navigation
        public ICollection<ApplicationUser> ReportingEmployees { get; set; }
       = new List<ApplicationUser>();

        public ICollection<ApplicationUser> SupervisedEmployees { get; set; }
            = new List<ApplicationUser>();







        [NotMapped]
        public bool IsSuperAdmin { get; set; }

        // For UI display only
        [NotMapped]
        public string SelectedRole { get; set; }


        [NotMapped]
        public List<string> SelectedRoles { get; set; } = new();        
    }
}