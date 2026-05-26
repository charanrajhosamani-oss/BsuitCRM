using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class RejectReasonMaster
{
    public Guid RejectReasonId { get; set; }

    public string ReasonName { get; set; } = null!;

    public string? ReasonCode { get; set; }

    public string ModuleType { get; set; } = null!;

    public string? StageType { get; set; }

    public int? DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public Guid? TenantId { get; set; }
}
