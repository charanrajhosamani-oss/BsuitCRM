
#nullable disable
namespace BSuit.API.Areas.Admin.Models
{
    // Areas/Admin/Models/ChangePasswordVM.cs
    public class ChangePasswordVM
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string UserName { get; set; }
    }
}
