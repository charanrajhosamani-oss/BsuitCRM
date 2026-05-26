
#nullable disable
namespace BSuit.Core.Entities
{
    public class TenantSubscriptionModule:_BASE2
    {
        public Guid TenantSubscriptionId { get; set; }
        public int ModuleId { get; set; }

        public TenantSubscription TenantSubscription { get; set; }
        public ModuleMaster Module { get; set; }
    }
}
