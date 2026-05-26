
#nullable disable
namespace BSuit.Core.Entities
{
    public class TenantSubscription: _BASE2
    {
        public Guid TenantId { get; set; }
        public int SubscriptionId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public TenantMaster Tenant { get; set; }
        public SubscriptionMaster Subscription { get; set; }

        public ICollection<TenantSubscriptionModule> Modules { get; set; }
    }
}
