using BSuit.API.Areas.Admin.Models.Base;
using BSuit.Identity.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

#nullable disable
namespace BSuit.API.Areas.Admin.Models
{
    // Areas/Admin/Models/AdminUserVM.cs
    /// <summary>
    /// ViewModel (Paging + Filter)
    /// </summary>
    public class AdminUserVM : GridRequest
    {
        public List<ApplicationUser> Users { get; set; }


        public List<SelectListItem> Roles { get; set; } = new();
       
    }
}
