using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class WinLossReason
{
    public Guid WinLossReasonId { get; set; }

    public string? ReasonType { get; set; }

    public string? ReasonName { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public Guid? TenantId { get; set; }
}
