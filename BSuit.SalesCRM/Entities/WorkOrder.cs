using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class WorkOrder
{
    public Guid WorkOrderId { get; set; }

    public Guid OpportunityId { get; set; }

    public string? WorkOrderNumber { get; set; }

    public string? WorkOrderURL { get; set; }

    public string? Description { get; set; }

    public Guid? ServiceId { get; set; }

    public DateTime? WOSentOn { get; set; }

    public bool? WOAccepted { get; set; }

    public DateTime? WOAcceptedOn { get; set; }

    public string? ReasonForWONonAccepence { get; set; }

    public decimal? EstimatedHours { get; set; }

    public decimal? ActualHours { get; set; }

    public DateTime? ProjectStartDate { get; set; }

    public DateTime? ProjectEndDate { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? TenantId { get; set; }

    public string? WorkOrderStatus { get; set; }

    public bool? FinanceApproved { get; set; }

    public Guid? FinanceApprovedBy { get; set; }

    public DateTime? FinanceApprovedOn { get; set; }

    public string? PaymentLinkURL { get; set; }

    public bool? PaymentLinkSent { get; set; }

    public DateTime? PaymentLinkSentOn { get; set; }

    public bool? ProjectCreated { get; set; }

    public string? ApprovalToken { get; set; }

    public Guid? ProposalId { get; set; }

    public string? ClientEmail { get; set; }

    public string? ClientName { get; set; }

    public string? FinanceRemarks { get; set; }

    public string? PaymentStatus { get; set; }

    public DateTime? PaymentReceivedOn { get; set; }

    public DateTime? ProjectCreatedOn { get; set; }

    public DateTime? WORejectedOn { get; set; }

    public Guid? WOApprovedBy { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<WorkOrderActivity> WorkOrderActivities { get; set; } = new List<WorkOrderActivity>();
}
