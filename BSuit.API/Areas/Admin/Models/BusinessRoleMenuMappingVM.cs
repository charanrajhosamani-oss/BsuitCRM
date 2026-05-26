using BSuit.API.Areas.Admin.Models.Base;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BSuit.API.Areas.Admin.Models
{
    public class BusinessRoleMenuMappingVM: GridRequest
    {
        public int MenuId { get; set; }

        public string MenuName { get; set; }

        public Guid TenantId { get; set; }
       

        public string RoleId { get; set; }

        public List<RoleItemVM> Roles { get; set; } = new();

        public List<string> AssignedRoleIds { get; set; } = new();
    }

    public class RoleItemVM
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}