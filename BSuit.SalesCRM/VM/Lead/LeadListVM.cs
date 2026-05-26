using BSuit.SalesCRM.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSuit.SalesCRM.VM.Lead
{
    public class LeadListVM
    {
        // 🔹 All Leads (paged result)
        public List<LeadInfo> LeadList { get; set; } = new();

        // 🔥 Pagination (current tab)
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }   // 👉 For CURRENT selected stage only

        // 🔥 NEW: Stage-wise counts (for tabs)
        public Dictionary<string, int> StageCounts { get; set; } = new();

        // 🔥 NEW: Current selected stage
        public string SelectedStage { get; set; }

        public List<SelectListItem> SalesExecutiveList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> RejectReasonsList { get; set; } = new List<SelectListItem>();
    }

    public class LeadInfo
    {
        // 🔹 Identifiers
        public Guid LeadId { get; set; }
        public Guid TeanantId { get; set; }

        // 🔹 Basic Info
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string EnquiryId { get; set; } = string.Empty;
        public string? Gender { get; set; }
        public string? JobTitle { get; set; }

        // 🔹 Contact Info
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string PersonalEmail1 { get; set; } = string.Empty;
        public string PersonalEmail2 { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string? Country { get; set; }

        // 🔹 Company Info
        public string CompanyName { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string? CompanySize { get; set; }
        public string CompanyRevenue { get; set; } = string.Empty;
        public string CompanyRanking { get; set; } = string.Empty;
        public string? Industry { get; set; }

        // 🔹 Additional Info
        public string RequirementDetails { get; set; } = string.Empty;
        public string CustomerBackground { get; set; } = string.Empty;
        public string SkpeId { get; set; } = string.Empty;
        public string TwitterURL { get; set; } = string.Empty;
        public string FacebookURL { get; set; } = string.Empty;

        public string? Rating { get; set; }
        public string? LeadPriority { get; set; }
        public string? LeadType { get; set; }
        public string? LeadSource { get; set; }
        public string? LeadStage { get; set; }
        public string? SalesExecutive { get; set; }

        public string? RejectionReason { get; set; }
        public string? RejectionRemarks { get; set; }

        public bool IsLeadRejectionApproved { get; set; }

        public DateTime? LeadRejectionApprovedDate { get; set; }

        public string? LeadRejectionApprovedBy { get; set; }

        // 🔹 Status
        public bool IsActive { get; set; }

        public DateTime? LeadCreatedOn { get; set; }

        public List<LeadServiceList> SelectedServices { get; set; } = new();

        public List<Documents> LeadDocuments { get; set; } = new();

        public List<ClientCommunication> EmailHistory { get; set; } = new();

        public List<LeadOpportunities> LeadOpportunities { get; set; } = new();

        public string? NDALink { get; set; }

        public NDASignature? NDA_Details { get; set; }

        public List<LeadActivity> LeadActivities { get; set; } = new();

        public Guid? LeadOwnerId { get; set; }
    }

    public class LeadServiceList
    {
        public Guid LeadId { get; set; }
        public Guid ServiceId { get; set; }
        public string? ServiceName { get; set; }
    }


    public class Documents
    {
        public string? FileName { get; set; }
        public DateTime UploadedOn { get; set; }

        public string? FilePath { get; set; }
    }

    public class ClientCommunication
    {
        public Guid EmailId { get; set; }

        public string? Subject { get; set; }

        public string? BodyPreview { get; set; }

        public DateTime? SentDate { get; set; }

        public string? Status { get; set; }

        // Recipients (for UI display)
        public string? ToEmails { get; set; }
        public string? CcEmails { get; set; }
        public string? BccEmails { get; set; }
    }

    public class LeadOpportunities
    {
        public Guid Id { get; set; }

        public Guid? WorkFlowId { get; set; }

        public Guid ServiceId { get; set; }

        public string ServiceName { get; set; }

        public string? Name { get; set; }

        public string? OpportunityStage { get; set; }

        public string? Scoper { get; set; }

        public DateTime RequestedOn { get; set; }

        public DateTime? SubmittedOn { get; set; }

        public DateTime? ApprovedOn { get; set; }

        public string? DeliveryManager { get; set; }
    }

    public class LeadActivity
    {
        public string Type { get; set; } = "Note";

        public string Description { get; set; } = "Test Description";

        public string? CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
