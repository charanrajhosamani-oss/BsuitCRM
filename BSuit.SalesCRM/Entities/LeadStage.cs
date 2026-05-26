using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class LeadStage
{
    public Guid LeadStageId { get; set; }

    public string? StageName { get; set; }

    public string? Description { get; set; }

    public int? DisplayOrder { get; set; }

    public bool? IsActive { get; set; }

    public Guid? TenantId { get; set; }

    public virtual ICollection<Lead> Leads { get; set; } = new List<Lead>();
}
