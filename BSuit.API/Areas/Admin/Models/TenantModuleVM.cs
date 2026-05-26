
#nullable disable
namespace BSuit.API.Areas.Admin.Models
{
    // Areas/Admin/Models/TenantModuleVM.cs
    public class TenantModuleVM
    {
        public Guid TenantId { get; set; }
        public string TenantName { get; set; }

        public List<ModuleCheckbox> Modules { get; set; }
    }

    public class ModuleCheckbox
    {
        public int ModuleId { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}
