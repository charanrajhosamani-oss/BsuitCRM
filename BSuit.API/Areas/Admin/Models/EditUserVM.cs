namespace BSuit.API.Areas.Admin.Models
{
    public class EditUserVM
    {
        public string Id { get; set; }
        public Guid TenantId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }      
        public List<string> SelectedRoleIds { get; set; } = new();
        public string Password { get; set; }

        public string? ReportingManagerId { get; set; }

        public string? SupervisorId { get; set; }
    }
}
