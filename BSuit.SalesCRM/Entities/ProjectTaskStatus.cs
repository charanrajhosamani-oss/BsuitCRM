using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class ProjectTaskStatus
{
    public Guid StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public int? DisplayOrder { get; set; }

    public bool? IsClosed { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? TenantId { get; set; }

    public virtual ICollection<ProjectTask> ProjectTasks { get; set; } = new List<ProjectTask>();
}
