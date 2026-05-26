using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class ProposalActivity
{
    public Guid ActivityId { get; set; }

    public Guid? ProposalId { get; set; }

    public string? ActivityType { get; set; }

    public string? Remarks { get; set; }

    public Guid? ActionBy { get; set; }

    public DateTime? ActionDate { get; set; }

    public Guid? TenantId { get; set; }
}
