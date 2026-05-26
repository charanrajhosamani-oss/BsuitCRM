using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class ApprovalWorkflowLevelUser
{
    public Guid WorkflowLevelUserId { get; set; }

    public Guid WorkflowLevelId { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public virtual ApprovalWorkflowLevel WorkflowLevel { get; set; } = null!;
}
