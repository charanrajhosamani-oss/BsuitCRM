using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSuit.SalesCRM.VM.Lead
{
    public class ServicePricingVM
    {
        public List<ServicePricingInfo> ServicePricingList { get; set; } = new();

        public List<ServiceInfo> ServiceList { get; set; } = new();

        public List<RegionMaster> Regions { get; set; } = new();

        public List<CurrencyInfo> CurrencyList { get; set; } = new();
    }

    public class ServicePricingInfo
    {
        public Guid ServicePricingId { get; set; }

        public Guid ServiceId { get; set; }

        public decimal LowPrice { get; set; }

        public decimal HighPrice { get; set; }

        public bool IsActive { get; set; }

        public Guid CurrencyId { get; set; }

        public Guid RegionId { get; set; }

    }

    public class ServiceInfo
    {
        public Guid ServiceId { get; set; }

        public string ServiceName { get; set; }
    }

    public class RegionMaster
    {
        public Guid RegionId { get; set; }

        public string RegionName { get; set; }
    }

    public class CurrencyInfo
    {
        public Guid CurrencyId { get; set; }

        public string CurrencyCode { get; set; }
    }
}
