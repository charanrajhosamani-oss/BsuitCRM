using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class ProjectTask
{
    public Guid ProjectTaskId { get; set; }

    public Guid ProjectId { get; set; }

    public Guid ProjectModuleId { get; set; }

    public string TaskName { get; set; } = null!;

    public string? Description { get; set; }

    public Guid? AssignedTo { get; set; }

    public Guid? TaskStatusId { get; set; }

    public Guid? PriorityId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? DueDate { get; set; }

    public decimal? EstimatedHours { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? TenantId { get; set; }

    public virtual ProjectTaskPriority? Priority { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual ICollection<ProjectTimeLog> ProjectTimeLogs { get; set; } = new List<ProjectTimeLog>();

    public virtual ProjectTaskStatus? TaskStatus { get; set; }
}
