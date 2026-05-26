using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class ProjectTimeLog
{
    public Guid TimeLogId { get; set; }

    public Guid? ProjectTaskId { get; set; }

    public Guid? UserId { get; set; }

    public DateOnly? WorkDate { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int? HoursSpent { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? TenantId { get; set; }

    public virtual ProjectTask? ProjectTask { get; set; }
}
