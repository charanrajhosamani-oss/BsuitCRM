using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class LeadConfiguration
{
    public Guid LeadConfigId { get; set; }

    public Guid SalesExecutiveId { get; set; }

    public Guid RegionId { get; set; }

    public Guid ServiceId { get; set; }

    public string? Remarks { get; set; }

    public Guid? RuleId { get; set; }

    public DateTime? EffectiveFrom { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? TenantId { get; set; }

    public Guid? SecondarySalesExecutiveId { get; set; }

    public virtual Region Region { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
