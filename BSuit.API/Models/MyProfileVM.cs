namespace BSuit.API.Models
{
    public class MyProfileVM
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }

        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }



        // Tenant Info
        public Guid TenantId { get; set; }
        public string TenantName { get; set; }
        public string TenantEmail { get; set; }
        public string DomainName { get; set; }
        public DateTime CreatedOn { get; set; }
        public string TenantStatus { get; set; }

    }
}
