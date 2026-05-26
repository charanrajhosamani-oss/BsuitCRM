
using BSuit.API.Areas.Admin.Models.Base;

#nullable disable
namespace BSuit.API.Areas.Admin.Models
{
    // Areas/Admin/Models/TenantVM.cs
    public class TenantVM : GridRequest
    {
        public List<Core.Entities.TenantMaster> Items { get; set; }      
    }
}
