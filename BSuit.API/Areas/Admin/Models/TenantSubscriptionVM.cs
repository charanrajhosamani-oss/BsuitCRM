using BSuit.API.Areas.Admin.Models.Base;
using BSuit.Core.Entities;

#nullable disable
namespace BSuit.API.Areas.Admin.Models
{
    // Areas/Admin/Models/TenantSubscriptionVM.cs
    public class TenantSubscriptionVM : GridRequest
    {
        public List<Core.Entities.TenantSubscription> Items { get; set; }

        public List<TenantMaster> Tenants { get; set; }
        public List<Core.Entities.SubscriptionMaster> Subscriptions { get; set; }

        public Guid TenantId { get; set; }
    }
}
