using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class ApprovalWorkflowRole
{
    public Guid WorkflowRoleId { get; set; }

    public Guid WorkflowId { get; set; }

    public string RoleId { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public virtual ApprovalWorkflowMaster Workflow { get; set; } = null!;
}
