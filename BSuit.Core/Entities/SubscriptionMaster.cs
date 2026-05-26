
#nullable disable
namespace BSuit.Core.Entities
{
    public class SubscriptionMaster: _BASE
    {    
        public string Name { get; set; }   // Basic, Pro, Enterprise
        public decimal Price { get; set; }

        public int DurationInDays { get; set; }

        public ICollection<TenantSubscription> TenantSubscriptions { get; set; }
    }
}
