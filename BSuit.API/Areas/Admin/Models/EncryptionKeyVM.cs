using BSuit.API.Areas.Admin.Models.Base;
using BSuit.Core.Entities;

#nullable disable
namespace BSuit.API.Areas.Admin.Models
{
    public class EncryptionKeyVM : GridRequest
    {
        public List<EncryptionKey> Items { get; set; }

        public List<TenantMaster> Tenants { get; set; }

    }
}
