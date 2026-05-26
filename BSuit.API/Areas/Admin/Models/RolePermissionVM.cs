using BSuit.API.Areas.Admin.Models.Base;

namespace BSuit.API.Areas.Admin.Models
{
    public class RolePermissionVM:GridRequest
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }

        public Guid? TenantId { get; set; }

        public List<RoleDropdownVM> Roles { get; set; } = new();
        public List<PermissionGroupVM> PermissionGroups { get; set; } = new();

        public List<AssignedPermissionVM> AssignedPermissions { get; set; } = new();

       
    }

    public class RoleDropdownVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class PermissionGroupVM
    {
        public string ModuleName { get; set; }
        public List<PermissionItemVM> Permissions { get; set; } = new();
    }

    public class PermissionItemVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }

        public bool IsAssigned { get; set; }
    }

    public class AssignedPermissionVM
    {
        public int PermissionId { get; set; }
        public string ModuleName { get; set; }
        public string DisplayName { get; set; }

        public string RoleName { get; set; }
    }
}
