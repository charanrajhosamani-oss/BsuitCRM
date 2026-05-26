using BSuit.SalesCRM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSuit.SalesCRM.VM._Opportunity_
{
    public class OpportunityVM
    {
        // 🔥 ADD THIS
        public string PendingRole { get; set; }
        public string ApprovalStatus { get; set; }
        public string ApprovedRole { get; set; }

        public string ApprovedByName { get; set; }
        public string NextApprovalStatus { get; set; }

        public string NextApprovalRole { get; set; }
        public List<EditServiceVM>  ViewServices { get; set; }
        public List<OpportunityServiceVM> EditServices { get; set; }
        public List<int> ServiceIds { get; set; }
        public Guid OpportunityId { get; set; }
        public Guid? LeadId { get; set; }
        public Guid? ServiceId { get; set; }
        public Guid? AccountId { get; set; }
        public string LeadName { get; set; }
        public string ServiceName { get; set; }
        public string? StageName { get; set; }
        public string LeadEmail { get; set; }
        
                    public string ScoperStatus { get; set; }

        public string ModuleName { get; set; }
        public List<OpportunityDetailVM> Details { get; set; }
        public string OpportunityName { get; set; }

        public string OpportunityStage { get; set; }
        public string Scoper { get; set; }
        public string SlaesExecutive { get; set; }

        public string ScopeofWork { get; set; }
        public string FinalDeliverableUsage { get; set; }
        public string ExpectationSetting { get; set; }
        public string CustomerResponsibilities { get; set; }
        public string EnquiryId { get; set; }
        public string CustomerSupport { get; set; }
        public int TurnHoursDays { get; set; }

        public int  NoOfMonth { get; set; }

        public string CustomerId { get; set; }

        public EditServiceVM SingleService { get; set; }
        public decimal? ExecutionHours { get; set; }
        public decimal? QCHours { get; set; }
        public decimal? TotalEstimationHours { get; set; }

        public DateTime? EstimatedOn { get; set; }

        public Guid? ScopeAnalystId { get; set; }
        public Guid? EngagementModelId { get; set; }
        public Guid? OpportunityStageId { get; set; }
        public Guid? DealTypeId { get; set; }
        public Guid? WinLossReasonId { get; set; }

        public DateTime? CreatedOn { get; set; }

        public bool IsSubmit { get; set; }

        public List<ServiceVM> Services { get; set; }  // 🔥 NEW

    }
    public class ViewServiceVM
    {
        public Guid OpportunityServiceId { get; set; }

        public Guid ServiceId { get; set; }
        public string ServiceName { get; set; }

        public string ScopeofWork { get; set; }
        public string FinalDeliverableUsage { get; set; }
        public string ExpectationSetting { get; set; }
        public string CustomerResponsibilities { get; set; }

        public string StageName { get; set; }

        // 🔹 Child list (Modules under this service)
        public List<ViewDetailVM> Details { get; set; } = new();
    }
    public class ViewDetailVM
    {
        public string ModuleName { get; set; }

        public decimal? EstimatedHours { get; set; }

        public decimal? QCHours { get; set; }
    }
    public class OpportunityListVM
    {

        public List<int> ServiceIds { get; set; }
        public List<OpportunityVM> Opportunities { get; set; } = new List<OpportunityVM>();
        public Dictionary<string, int> StageCounts { get; set; }
        public string SelectedStage { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class OpportunityDetailVM
    {
        public string ModuleName { get; set; }
        public decimal? ExecutionHours { get; set; }
        public decimal? QCHours { get; set; }
        public DateTime? EstimatedOn { get; set; }

        public decimal? EstimatedHours { get; set; }

    }

    public class ServiceVM
    {
        // 🔥 ADD THESE TWO
        public string PendingRole { get; set; }
        public string ApprovalStatus { get; set; }
        public Guid ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string ScoperStatus { get; set; }
        public string ScopeofWork { get; set; }
    }

    public class OpportunityServiceVM
    {
        public Guid ServiceId { get; set; }

        public Guid? LeadId { get; set; }
        public Guid? AccountId { get; set; }
        public string ScopeofWork { get; set; }

        public string ServiceName { get; set; }
        public string FinalDeliverableUsage { get; set; }
        public string ExpectationSetting { get; set; }
        public string CustomerResponsibilities { get; set; }

        public Guid? ScopeAnalystId { get; set; }
        public Guid? EngagementModelId { get; set; }
        public Guid? OpportunityStageId { get; set; }

        public Guid? IndustryId { get; set; }

        public Guid? DealTypeId { get; set; }
        public Guid? WinLossReasonId { get; set; }

        public List<OpportunityDetailVM> Details { get; set; }

        public string CustomerSupport { get; set; }
        public int TurnHoursDays { get; set; }

        public int NoOfMonth { get; set; }
    }

    public class EditServiceVM
    {
        public Guid OpportunityServiceId { get; set; }   // 🔥 Required for mapping modules
        public Guid ServiceId { get; set; }

                    public Guid? OpportunityStageId
        { get; set; }

        public string ServiceName { get; set; }
        public string EngagementModelName { get; set; }

        
        public string ScopeofWork { get; set; }
        public string FinalDeliverableUsage { get; set; }
        public string ExpectationSetting { get; set; }
        public string CustomerResponsibilities { get; set; }
        public string IndustryName { get; set; }
        
        public string ApprovalStatus { get; set; }

        public string CurrencyCode { get; set; }

        public string StageName { get; set; }
        
        public List<DetailVM> Details { get; set; } = new List<DetailVM>();

        public string CustomerSupport { get; set; }
        public int TurnHoursDays { get; set; }

        public int NoOfMonth { get; set; }
    }

    public class DetailVM
    {
        public Guid OpportunityModuleId { get; set; }   // optional but useful for edit/delete

        public string ModuleName { get; set; }

        public decimal? EstimatedHours { get; set; }

        public decimal? QCHours { get; set; }
    }


    public class ApprovalInboxVM
    {
        public Guid OpportunityId { get; set; }
        public string OpportunityName { get; set; }
        public DateTime? SubmittedOn { get; set; }

        public Guid ApprovalId { get; set; }

        public string ApprovalStatus { get; set; }

        public List<ApprovalServiceVM> Services { get; set; }
    }

    public class ApprovalServiceVM
    {
        public Guid? ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string ApprovalStatus { get; set; }
    }
    public class ApprovalOpportunityVM
    {

        public Guid? WorkOrderId { get; set; }

        public Guid? ProposalId { get; set; }

        public string WorkOrderNumber { get; set; }
        public string WorkOrderStatus { get; set; }
        public bool HasWorkOrder { get; set; }
        public string ApprovedByName { get; set; }
        public DateTime? LastApprovedActionDate { get; set; }
        public Guid? ServiceId { get; set; }

        public Guid? StageId { get; set; }
        public Guid ApprovalId { get; set; }

        public Guid OpportunityId { get; set; }

        public Guid OpportunityServiceId { get; set; }

        public string OpportunityName { get; set; }

        public string LeadName { get; set; }

        public string ServiceName { get; set; }

        public string StageName { get; set; }

        public string RoleId { get; set; }


        public string ScoperStatus { get; set; }
        public string Scoper { get; set; }
        public string ApprovalStatus { get; set; }
        public string ApprovedRole { get; set; }
        public string SlaesExecutive { get; set; }
        public string SubmittedBy { get; set; }

        public DateTime? SubmittedOn { get; set; }
        public DateTime? EstimatedOn { get; set; }

    }
    public class ApprovalOpportunityListVM
    {
        public int TotalCount { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public List<ApprovalOpportunityVM> Opportunities { get; set; }
    }


    public class ApprovalWorkflowVM
    {
        public string ModuleName { get; set; }

        public string WorkflowName { get; set; }

        // MULTIPLE SERVICES
        public List<Guid> SelectedServiceIds { get; set; }

        // MULTIPLE ROLES
        public List<string> SelectedRoleIds { get; set; }

        // LEVELS
        public List<ApprovalLevelVM> Levels { get; set; }
    }

    public class ApprovalLevelVM
    {
        public int LevelNo { get; set; }

        // MULTIPLE USERS
        public List<string> UserIds { get; set; }

        public bool IsFinalLevel { get; set; }
    }

    public class WorkOrderVM
    {
        // =========================
        // WORK ORDER BASIC
        // =========================
        public Guid WorkOrderId { get; set; }
        public string WorkOrderNumber { get; set; }
        public string Description { get; set; }
        public string WorkOrderStatus { get; set; }
        public decimal? EstimatedHours { get; set; }
        public decimal? ActualHours { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public bool IsExecuted { get; set; }
        public string NonAcceptanceReason { get; set; }
        public string WorkOrderURL
        { get; set; }

        // =========================
        // CUSTOMER (LEAD)
        // =========================
        public string CustomerName { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerCountry { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string CustomerBackground
        { get; set; }
        public string EngagementModelName

        { get; set; }
        public string BusinessType
        { get; set; }
        public string ProjectIndustry
        { get; set; }

        // =========================
        // PROJECT DETAILS
        // =========================
        public string ServiceName { get; set; }
        public string IndustryName { get; set; }

        public string ScopeOfWork { get; set; }
        public string FinalDeliverable { get; set; }
        public string ExpectationSetting { get; set; }
        public string CustomerResponsibilities { get; set; }

        // =========================
        // PROPOSAL
        // =========================

        // =========================
        // OPPORTUNITY
        // =========================
        public Guid? OpportunityId { get; set; }

        public string? OpportunityName { get; set; }
        public Guid? ProposalId { get; set; }

        public string? ProposalNumber { get; set; }

        public string? ProposalStatus { get; set; }

        public decimal? ProposedAmount { get; set; }

        public string? CurrencyCode { get; set; }
    }

    public class UpdateProposalVM
    {
        public Guid OpportunityId { get; set; }

        public Guid ServiceId { get; set; }

        public decimal EstimatedHours { get; set; }

        public decimal Rate { get; set; }

        public decimal ProposedAmount { get; set; }
    }

    public class ProposalVM
    {
        public Guid ProposalId { get; set; }

        public string ProposalNumber { get; set; }

        public string ProposalTitle { get; set; }

        public string ProposalStatus { get; set; }

        public string ProposalURL { get; set; }

        public decimal? EstimatedHours { get; set; }

        public decimal? ProposedAmount { get; set; }

        public string CurrencyCode { get; set; }

        public DateTime? CreatedOn { get; set; }

        // ======================================
        // CUSTOMER
        // ======================================

        public string CustomerName { get; set; }

        public string CustomerCity { get; set; }

        public string CustomerCountry { get; set; }

        public string CompanyName { get; set; }

        public string Address { get; set; }

        public string CustomerBackground { get; set; }

        public string BusinessType { get; set; }

        // ======================================
        // PROJECT
        // ======================================

        public string OpportunityName { get; set; }

        public string ServiceName { get; set; }

        public string IndustryName { get; set; }

        public string ProjectIndustry { get; set; }

        public string ScopeOfWork { get; set; }

        public string FinalDeliverable { get; set; }

        public string ExpectationSetting { get; set; }

        public string CustomerResponsibilities { get; set; }
    }
}
