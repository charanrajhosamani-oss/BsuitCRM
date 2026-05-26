using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class WorkOrderActivity
{
    public Guid ActivityId { get; set; }

    public Guid WorkOrderId { get; set; }

    public string? ActivityType { get; set; }

    public string? Remarks { get; set; }

    public Guid? ActionBy { get; set; }

    public DateTime? ActionDate { get; set; }

    public Guid? TenantId { get; set; }

    public string? OldStatus { get; set; }

    public string? NewStatus { get; set; }

    public string? ActivityByRole { get; set; }

    public virtual WorkOrder WorkOrder { get; set; } = null!;
}
