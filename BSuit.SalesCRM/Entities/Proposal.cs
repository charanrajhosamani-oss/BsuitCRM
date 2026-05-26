using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class Proposal
{
    public Guid ProposalId { get; set; }

    public Guid OpportunityId { get; set; }

    public Guid? ServiceId { get; set; }

    public string? ProposalNumber { get; set; }

    public string? ProposalURL { get; set; }

    public string? ProposalTitle { get; set; }

    public string? ScopeOfWork { get; set; }

    public string? Deliverables { get; set; }

    public decimal? EstimatedHours { get; set; }

    public decimal? Rate { get; set; }

    public decimal? ProposedAmount { get; set; }

    public Guid? CurrencyId { get; set; }

    public DateTime? SentOn { get; set; }

    public bool? ClientApproved { get; set; }

    public DateTime? ClientApprovedOn { get; set; }

    public string? ClientRejectedReason { get; set; }

    public string? ApprovalToken { get; set; }

    public string? Description { get; set; }

    public string? ProposalStatus { get; set; }

    public int? VersionNo { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? TenantId { get; set; }
}
