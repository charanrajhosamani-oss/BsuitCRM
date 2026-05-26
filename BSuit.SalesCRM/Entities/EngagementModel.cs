using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class EngagementModel
{
    public Guid EngagementModelId { get; set; }

    public string EngagementModelName { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? TenantId { get; set; }
}
