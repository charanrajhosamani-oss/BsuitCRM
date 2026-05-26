using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class Currency
{
    public Guid CurrencyId { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public string CurrencyName { get; set; } = null!;

    public string? CurrencySymbol { get; set; }

    public bool? IsBaseCurrency { get; set; }

    public decimal? ExchangeRate { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public Guid? TenantId { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<ServicePricing> ServicePricings { get; set; } = new List<ServicePricing>();
}
