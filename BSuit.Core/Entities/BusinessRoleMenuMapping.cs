
namespace BSuit.Core.Entities
{
    public class BusinessRoleMenuMapping : _BASE
    {
        public string RoleId { get; set; }

        public int MenuId { get; set; }

        public Guid? TenantId { get; set; }
        public int ModuleId { get; set; }
        public string Remarks { get; set; }

        public virtual BusinessMenuMaster Menu { get; set; }
    }
}
