
#nullable disable
namespace BSuit.Core.Entities
{
    public class RolePermission:_BASE2
    {
        public string RoleId { get; set; }   // Identity RoleId
        public int PermissionId { get; set; }

        public Guid? TenantId { get; set; } = Guid.Empty;

        public PermissionMaster Permission { get; set; }
    }
}
