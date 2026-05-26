using BSuit.API.Areas.Admin.Models.Base;
using BSuit.Core.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BSuit.API.Areas.Admin.Models
{
    public class BusinessMenuMasterVM: GridRequest
    {
        public bool IsSuperAdmin { get; set; }

        public int Id { get; set; }

        [Required]
        public Guid TenantId { get; set; }

        [Required]
        [Display(Name = "Module")]
        public int ModuleId { get; set; }

        public int? ParentMenuId { get; set; }

        [Required]
        [Display(Name = "Menu Name")]
        public string MenuName { get; set; }

        public string Url { get; set; }

        public string Icon { get; set; }

        public int SortOrder { get; set; }

        public string Remarks { get; set; }

        public bool IsActive { get; set; }

        public string ModuleName { get; set; }

        public string ParentMenuName { get; set; }

        public IEnumerable<SelectListItem> Modules { get; set; }

        public IEnumerable<SelectListItem> ParentMenus { get; set; }

        public List<BusinessMenuMaster> Items { get; set; }
            = new();
        public List<SelectListItem> Tenants { get; set; }
        
    }
}
