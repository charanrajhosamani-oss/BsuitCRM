using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class LeadType
{
    public Guid LeadTypeId { get; set; }

    public string LeadTypeName { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public int? DisplayOrder { get; set; }

    public DateTime? CreatedDate { get; set; }

    public Guid? TenantId { get; set; }

    public virtual ICollection<Lead> Leads { get; set; } = new List<Lead>();
}
