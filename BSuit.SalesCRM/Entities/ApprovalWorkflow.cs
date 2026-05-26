using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class ApprovalWorkflow
{
    public Guid WorkflowId { get; set; }

    public string? ModuleName { get; set; }

    public string? WorkflowName { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<ApprovalTransaction> ApprovalTransactions { get; set; } = new List<ApprovalTransaction>();

    public virtual ICollection<WorkflowStep> WorkflowSteps { get; set; } = new List<WorkflowStep>();
}
