using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class LeadActivity
{
    public Guid ActivityId { get; set; }

    public Guid? LeadId { get; set; }

    public string? ActivityType { get; set; }

    public string? Notes { get; set; }

    public DateTime? ActivityDate { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? TenantId { get; set; }
}
