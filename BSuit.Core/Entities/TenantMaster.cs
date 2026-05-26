
#nullable disable
namespace BSuit.Core.Entities
{
    public class TenantMaster:_BASE2
    {        
        public string Name { get; set; }
        public string? Domain { get; set; }
        public string Email { get; set; }

        public ICollection<TenantSubscription> Subscriptions { get; set; }
        
    }
}
