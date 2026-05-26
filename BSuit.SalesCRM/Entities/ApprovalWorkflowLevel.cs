using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class ApprovalWorkflowLevel
{
    public Guid WorkflowLevelId { get; set; }

    public Guid WorkflowId { get; set; }

    public int LevelNo { get; set; }

    public string UserId { get; set; } = null!;

    public bool IsFinalLevel { get; set; }

    public DateTime CreatedOn { get; set; }

    public virtual ICollection<ApprovalWorkflowLevelUser> ApprovalWorkflowLevelUsers { get; set; } = new List<ApprovalWorkflowLevelUser>();

    public virtual ApprovalWorkflowMaster Workflow { get; set; } = null!;
}
