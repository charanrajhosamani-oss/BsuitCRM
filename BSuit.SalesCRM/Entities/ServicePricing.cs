using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class ServicePricing
{
    public Guid ServicePricingId { get; set; }

    public Guid ServiceId { get; set; }

    public decimal LowPrice { get; set; }

    public decimal HighPrice { get; set; }

    public bool? IsActive { get; set; }

    public Guid? CurrencyId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public Guid? TenantId { get; set; }

    public Guid? RegionId { get; set; }

    public virtual Currency? Currency { get; set; }

    public virtual Service Service { get; set; } = null!;
}
