using Microsoft.AspNetCore.Mvc.Rendering;

namespace BSuit.API.Areas.Admin.Models
{
    public class UserServiceMappingVM
    {
        public Guid TenantId { get; set; }
        public string TenantName { get; set; }

        public string SelectedRoleId { get; set; }
        public List<SelectListItem> Roles { get; set; } = new();

        // NEW
        public List<string> SelectedUserIds { get; set; } = new();
        public List<SelectListItem> Users { get; set; } = new();

        public List<Guid> SelectedServiceIds { get; set; } = new();
        public List<SelectListItem> Services { get; set; } = new();

        public List<MappedServiceVM> ExistingMappings { get; set; } = new();
    }

    public class MappedServiceVM
    {
        public Guid ServiceId { get; set; }
        public Guid RoleId { get; set; }
        public Guid UserId { get; set; }

        public string UserName { get; set; }
        public string ServiceName { get; set; }
        public string RoleName { get; set; }
        public DateTime AssignedOn { get; set; }
    }
}