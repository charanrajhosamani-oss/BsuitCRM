using BSuit.SalesCRM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSuit.SalesCRM.Models
{
    public class FinanceVM
    {
        public List<PaymentMilestoneDetails> payments { get; set; } = new List<PaymentMilestoneDetails>();

        public List<PaymentType> PaymentTypes { get; set; } = new List<PaymentType>();

    }

    public class PaymentMilestoneDetails
    {
        // Opportunity Details
        public string OpportunityName { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;

        // Work Order / Payment
        public Guid WorkOrderId { get; set; }
        public Guid PaymentId { get; set; }

        // Payment Details
        public string PaymentType { get; set; } = string.Empty;
        public string PaymentMilestone { get; set; } = string.Empty;

        public decimal InvoiceAmount { get; set; }

        public DateTime? PaymentDueDate { get; set; }

        public string PaymentLink { get; set; } = string.Empty;

        public string WorkOrderLink { get; set; } = string.Empty;

        // Hours & Rate
        public double Hours { get; set; }

        public double HourlyRate { get; set; }

        // Remarks
        public string BDRemarks { get; set; } = string.Empty;

        // Audit
        public string MilestoneCreatedBy { get; set; } = string.Empty;

        public DateTime MileStoneCreatedOn { get; set; }

        // ================================
        // Finance Team Update Properties
        // ================================

        // Transaction Details
        public string TransactionNumber { get; set; } = string.Empty;

        // Payment Received Date & Time
        public DateTime? PaymentReceivedDateTime { get; set; }

        // Amount Received
        public decimal AmountReceived { get; set; }

        // Mode Of Payment
        public string ModeOfPayment { get; set; } = string.Empty;

        // Currency
        public string PaymentCurrency { get; set; } = string.Empty;

        // Finance Remarks
        public string FinanceRemarks { get; set; } = string.Empty;

        // Approval Status
        public string FinanceStatus { get; set; } = string.Empty;

        // Approved / Rejected By
        public string FinanceActionBy { get; set; } = string.Empty;

        // Approved / Rejected On
        public DateTime? FinanceActionDate { get; set; }
    }
}
