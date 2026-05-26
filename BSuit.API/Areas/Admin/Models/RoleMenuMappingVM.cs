using BSuit.API.Areas.Admin.Models.Base;
using BSuit.Core.Entities;
using BSuit.Identity.Models;

#nullable disable
namespace BSuit.API.Areas.Admin.Models
{
    public class RoleMenuMappingVM: GridRequest
    {
        // GRID
        public List<ApplicationRole> Roles { get; set; } = new();

   
        // ASSIGN
        public string SelectedRoleId { get; set; }
        public List<int> SelectedMenuIds { get; set; } = new();

        public List<Menu> AllMenus { get; set; } = new();
        public List<int> AssignedMenuIds { get; set; } = new();


        public int? MenuId { get; set; }   //NEW
        public string MenuName { get; set; }
    }
}
