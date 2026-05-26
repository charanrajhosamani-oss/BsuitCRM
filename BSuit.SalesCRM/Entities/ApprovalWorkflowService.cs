using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class ApprovalWorkflowService
{
    public Guid WorkflowServiceId { get; set; }

    public Guid WorkflowId { get; set; }

    public Guid ServiceId { get; set; }

    public DateTime CreatedOn { get; set; }

    public virtual ApprovalWorkflowMaster Workflow { get; set; } = null!;
}
