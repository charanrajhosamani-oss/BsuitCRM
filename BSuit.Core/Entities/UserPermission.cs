
#nullable disable
namespace BSuit.Core.Entities
{
    public class UserPermission : _BASE2
    {
        public string UserId { get; set; }
        public int PermissionId { get; set; }

        public Guid TenantId { get; set; }

        public bool IsAllowed { get; set; } = true; // 🔥 Allow / Deny

        public PermissionMaster Permission { get; set; }
    }
}
