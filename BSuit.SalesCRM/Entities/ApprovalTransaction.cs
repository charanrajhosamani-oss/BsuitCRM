using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class ApprovalTransaction
{
    public Guid ApprovalId { get; set; }

    public Guid? WorkflowId { get; set; }

    public Guid? RecordId { get; set; }

    public Guid? StepId { get; set; }

    public Guid? StageId { get; set; }

    public Guid? ApprovedBy { get; set; }

    public string? ApprovalStatus { get; set; }

    public string? Remarks { get; set; }

    public DateTime? ActionDate { get; set; }

    public virtual WorkflowStep? Step { get; set; }

    public virtual ApprovalWorkflow? Workflow { get; set; }
}
