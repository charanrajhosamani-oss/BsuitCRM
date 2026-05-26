using BSuit.API.Areas.Admin.Models.Base;

#nullable disable
namespace BSuit.API.Areas.Admin.Models
{
    // Areas/Admin/Models/SubscriptionVM.cs
    public class SubscriptionVM : GridRequest
    {
        public List<Core.Entities.SubscriptionMaster> Items { get; set; }

    }
}
