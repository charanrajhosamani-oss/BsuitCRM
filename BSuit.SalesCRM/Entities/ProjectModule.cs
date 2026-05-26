using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class ProjectModule
{
    public Guid ProjectModuleId { get; set; }

    public Guid? OpportunityId { get; set; }

    public Guid? ProjectId { get; set; }

    public string? ModuleName { get; set; }

    public string? Description { get; set; }

    public decimal? EstimatedHours { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? TenantId { get; set; }

    public virtual Opportunity? Opportunity { get; set; }

    public virtual Project? Project { get; set; }

    public virtual ICollection<ProjectMoudleEmployeemap> ProjectMoudleEmployeemaps { get; set; } = new List<ProjectMoudleEmployeemap>();
}
