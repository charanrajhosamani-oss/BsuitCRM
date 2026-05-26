using BSuit.Identity.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BSuit.API.Areas.Admin.Models
{
    public class TenantUsersVM
    {
        public Guid TenantId { get; set; }
        public string TenantName { get; set; }

        public List<ApplicationUser> Users { get; set; } = new();

        public List<SelectListItem> ReportingManagers { get; set; } = new();

        public List<SelectListItem> Supervisors { get; set; } = new();

        public List<SelectListItem> RoleList { get; set; } = new();

        // Create form values
        public string? ReportingManagerId { get; set; }
        public string? SupervisorId { get; set; }

        public List<string> SelectedRoleIds { get; set; } = new();
    }
}
