using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class Payment
{
    public Guid PaymentId { get; set; }

    public Guid? WorkOrderId { get; set; }

    public Guid? ProjectId { get; set; }

    public string? PaymentMilestone { get; set; }

    public Guid? PaymentTypeId { get; set; }

    public decimal? InvoiceAmount { get; set; }

    public DateTime? InvoiceDate { get; set; }

    public DateTime? PaymentDueDate { get; set; }

    public bool? IsPartialPayment { get; set; }

    public double? Hours { get; set; }

    public double? HourlyRate { get; set; }

    public string? PaymentLink { get; set; }

    public string? BDRemarks { get; set; }

    public DateTime? MilestoneCreatedOn { get; set; }

    public Guid? MilestoneCreatedBy { get; set; }

    public DateTime? InvoiceStartDate { get; set; }

    public DateTime? InvoiceEndDate { get; set; }

    public Guid? PaymentModeId { get; set; }

    public string? TransactionNumber { get; set; }

    public Guid? CurrencyId { get; set; }

    public decimal? AmountRecieved { get; set; }

    public DateTime? PaymentReceivedOn { get; set; }

    public Guid? PaymentStatusId { get; set; }

    public string? FinanceRemarks { get; set; }

    public Guid? ConfirmedBy { get; set; }

    public Guid? TenantId { get; set; }

    public bool? IsFinanceApproved { get; set; }

    public DateTime? FinanceApprovedOn { get; set; }

    public Guid? FinanceApprovedBy { get; set; }

    public bool? IsFinanceRequestSent { get; set; }

    public DateTime? FinanceRequestSentOn { get; set; }

    public Guid? FinanceRequestSentBy { get; set; }

    public virtual Currency? Currency { get; set; }

    public virtual PaymentStatus? PaymentStatus { get; set; }

    public virtual PaymentType? PaymentType { get; set; }

    public virtual Project? Project { get; set; }

    public virtual WorkOrder? WorkOrder { get; set; }
}
