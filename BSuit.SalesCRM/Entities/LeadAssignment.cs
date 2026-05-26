using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class LeadAssignment
{
    public Guid LeadAssignmentId { get; set; }

    public Guid? LeadId { get; set; }

    public Guid? AssignedTo { get; set; }

    public DateTime? AssignedDate { get; set; }

    public Guid? AssignedBy { get; set; }

    public Guid? TenantId { get; set; }

    public virtual Lead? Lead { get; set; }
}
