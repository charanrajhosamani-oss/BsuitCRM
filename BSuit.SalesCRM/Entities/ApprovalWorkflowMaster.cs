using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class ApprovalWorkflowMaster
{
    public Guid WorkflowId { get; set; }

    public string ModuleName { get; set; } = null!;

    public string WorkflowName { get; set; } = null!;

    public Guid TenantId { get; set; }

    public bool IsActive { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public virtual ICollection<ApprovalWorkflowLevel> ApprovalWorkflowLevels { get; set; } = new List<ApprovalWorkflowLevel>();

    public virtual ICollection<ApprovalWorkflowRole> ApprovalWorkflowRoles { get; set; } = new List<ApprovalWorkflowRole>();

    public virtual ICollection<ApprovalWorkflowService> ApprovalWorkflowServices { get; set; } = new List<ApprovalWorkflowService>();
}
