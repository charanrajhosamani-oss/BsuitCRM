using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class LeadServiceMapping
{
    public Guid LeadServiceId { get; set; }

    public Guid LeadId { get; set; }

    public Guid ServiceId { get; set; }

    public Guid? SubServiceId { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public Guid? TenantId { get; set; }

    public virtual Lead Lead { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;

    public virtual Service? SubService { get; set; }
}
