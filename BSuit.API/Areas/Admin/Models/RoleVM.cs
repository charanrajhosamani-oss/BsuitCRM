using BSuit.API.Areas.Admin.Models.Base;

#nullable disable
namespace BSuit.API.Areas.Admin.Models
{
    public class RolePageVM
    {
        public RoleVM Grid { get; set; }
        public List<RoleVM> Items { get; set; }
    }

    public class RoleVM : GridRequest
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public bool IsActive { get; set; }
        public bool IsSuperadmin { get; set; }

        public Guid TenantId { get; set; }

        // 🔽 For dropdown
        public List<TenantDropdownVM> Tenants { get; set; } = new();
        public bool CanEdit { get; internal set; }
        public bool IsSystemRole { get; internal set; }
    }

    public class TenantDropdownVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }


}
