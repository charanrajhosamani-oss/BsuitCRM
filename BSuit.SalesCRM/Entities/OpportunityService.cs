using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class OpportunityService
{
    public Guid OpportunityServiceId { get; set; }

    public Guid? OpportunityId { get; set; }

    public Guid? ServiceId { get; set; }

    public Guid? L2ServiceId { get; set; }

    public Guid? L3ServiceId { get; set; }

    public Guid? LeadId { get; set; }

    public Guid? AccountId { get; set; }

    public string? ScopeofWork { get; set; }

    public string? FinalDeliverableUsage { get; set; }

    public string? ExpectationSetting { get; set; }

    public string? CustomerResponsibilities { get; set; }

    public string? Assumptions { get; set; }

    public string? Dependency { get; set; }

    public string? Risks { get; set; }

    public string? constraints { get; set; }

    public Guid? ScopeAnalystId { get; set; }

    public Guid? ProjectIndustryId { get; set; }

    public Guid? EngagementModelId { get; set; }

    public Guid? OpportunityStageId { get; set; }

    public Guid? DealTypeId { get; set; }

    public Guid? WinLossReasonId { get; set; }

    public DateTime? ExpectedOn { get; set; }

    public Guid? ScoperId { get; set; }

    public DateTime? ScoperUpdatedOn { get; set; }

    public string? ScoperStatus { get; set; }

    public Guid? CurrentStepId { get; set; }

    public Guid? WorkflowId { get; set; }

    public string? ServiceName { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public Guid TenantId { get; set; }

    public Guid? WorkOrderId { get; set; }

    public Guid? ProposalId { get; set; }

    public string? SupportTime { get; set; }

    public int? TurnHoursDays { get; set; }

    public int? NoOfMonth { get; set; }
}
