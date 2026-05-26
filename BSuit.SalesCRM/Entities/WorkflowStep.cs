using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class WorkflowStep
{
    public Guid StepId { get; set; }

    public Guid? WorkflowId { get; set; }

    public string? RoleId { get; set; }

    public int? LevelOrder { get; set; }

    public bool? IsMandatory { get; set; }

    public bool? IsFinalLevel { get; set; }

    public virtual ICollection<ApprovalTransaction> ApprovalTransactions { get; set; } = new List<ApprovalTransaction>();

    public virtual ApprovalWorkflow? Workflow { get; set; }
}
