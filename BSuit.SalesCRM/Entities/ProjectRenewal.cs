using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class ProjectRenewal
{
    public Guid ProjectRenewalId { get; set; }

    public Guid ProjectId { get; set; }

    public DateOnly? RenewalStartDate { get; set; }

    public DateOnly? RenewalEndDate { get; set; }

    public decimal? RenewalAmount { get; set; }

    public Guid? CurrencyId { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? TenantId { get; set; }

    public virtual Currency? Currency { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Project Project { get; set; } = null!;
}
