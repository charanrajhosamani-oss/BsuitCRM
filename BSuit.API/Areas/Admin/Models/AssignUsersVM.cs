
#nullable disable
namespace BSuit.API.Areas.Admin.Models
{
    // Areas/Admin/Models/AssignUsersVM.cs
    public class AssignUsersVM
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }

        public List<UserRoleCheckbox> Users { get; set; }
    }

    public class UserRoleCheckbox
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public bool IsSelected { get; set; }
    }
}
