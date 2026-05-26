using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class Project
{
    public Guid ProjectId { get; set; }

    public Guid OpportunityId { get; set; }

    public Guid AccountId { get; set; }

    public string ProjectName { get; set; } = null!;

    public string? ProjectCode { get; set; }

    public string? CustomerID { get; set; }

    public string? Description { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public decimal? ProjectHours { get; set; }

    public decimal? BalanceHours { get; set; }

    public decimal? TransferHours { get; set; }

    public string? Notes { get; set; }

    public string? Attachment { get; set; }

    public int? TeamId { get; set; }

    public Guid? ProjectManagerId { get; set; }

    public Guid? ProjectSupervisorId { get; set; }

    public Guid? PrimaryResourceId { get; set; }

    public Guid? ProjectStatusId { get; set; }

    public Guid? PriorityId { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? TenantId { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Priority? Priority { get; set; }

    public virtual ICollection<ProjectModule> ProjectModules { get; set; } = new List<ProjectModule>();

    public virtual ProjectStatus? ProjectStatus { get; set; }

    public virtual ICollection<ProjectTask> ProjectTasks { get; set; } = new List<ProjectTask>();
}
