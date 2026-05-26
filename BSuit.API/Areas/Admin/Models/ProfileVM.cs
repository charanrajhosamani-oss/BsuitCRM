#nullable disable
namespace BSuit.API.Areas.Admin.Models
{
    // Areas/Admin/Models/ProfileVM.cs
    public class ProfileVM
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool Active { get; set; }
        public bool IsTwoFactorEnabled { get; set; }

        public ChangePasswordVM ChangePasswordVM { get; set; }
    }
}
