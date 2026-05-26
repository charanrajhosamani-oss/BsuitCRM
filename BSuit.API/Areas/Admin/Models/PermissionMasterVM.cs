#nullable disable
using BSuit.API.Areas.Admin.Models.Base;
using BSuit.Core.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BSuit.API.Areas.Admin.Models
{
    public class PermissionMasterVM: GridRequest
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }

        public int Action { get; set; }               // 🔥
        public string ActionName { get; set; }        // For display

        public bool IsActive { get; set; }

        public List<SelectListItem> Actions { get; set; } = new();

        public List<ModuleDropdownVM> Modules { get; set; } = new();
        public List<PermissionMaster> Items { get; internal set; }
    }

    public class ModuleDropdownVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
