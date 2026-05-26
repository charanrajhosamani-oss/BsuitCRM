namespace BSuit.API.Areas.Admin.Models
{   
    public class UserPermissionVM
    {
        public string UserId { get; set; }
        public string UserName { get; set; }

        public Guid? TenantId { get; set; }

        public List<UserDropdownVM> Users { get; set; } = new();
        public List<PermissionGroupVM> PermissionGroups { get; set; } = new();

        public List<UserAssignedPermissionVM> AssignedPermissions { get; set; } = new();
    }

    public class UserDropdownVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class UserAssignedPermissionVM
    {
        public int PermissionId { get; set; }
        public string ModuleName { get; set; }
        public string DisplayName { get; set; }
        public bool IsAllowed { get; set; }
    }
}
