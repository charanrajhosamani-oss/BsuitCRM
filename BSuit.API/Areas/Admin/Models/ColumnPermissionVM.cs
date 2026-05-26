namespace BSuit.API.Areas.Admin.Models
{
    public class ColumnPermissionVM
    {
        public string RoleId { get; set; }
        public string UserId { get; set; }   // ✅ ADD
        public Guid? TenantId { get; set; }


        public List<RoleDropdownVM> Roles { get; set; } = new();
        public List<UserDropdownVM> Users { get; set; } = new();
        public List<ColumnGroupVM> Groups { get; set; } = new();

        public List<ColumnAssignedVM> Assigned { get; set; } = new();
    }

    public class ColumnGroupVM
    {
        public string EntityName { get; set; }
        public List<ColumnItemVM> Columns { get; set; } = new();
    }

    public class ColumnItemVM
    {
        public string ColumnName { get; set; }

        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
    }

    public class ColumnAssignedVM
    {
        public string EntityName { get; set; }
        public string ColumnName { get; set; }

        public bool CanView { get; set; }
        public bool CanEdit { get; set; }


        //
        public string TenantName { get; internal set; }
        public string PermissionName { get; internal set; }
        public string UserName { get; internal set; }
    }
}
