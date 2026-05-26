using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class LeadPriority
{
    public Guid LeadPriorityId { get; set; }

    public string PriorityName { get; set; } = null!;

    public string? Description { get; set; }

    public string? ColorCode { get; set; }

    public int? DisplayOrder { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }
}
