using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class LeadSource
{
    public Guid LeadSourceId { get; set; }

    public string? SourceName { get; set; }

    public int? DisplayOrder { get; set; }

    public Guid? TenantId { get; set; }

    public virtual ICollection<Lead> Leads { get; set; } = new List<Lead>();
}
