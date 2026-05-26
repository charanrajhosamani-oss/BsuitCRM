using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class ProjectStatus
{
    public Guid StatusId { get; set; }

    public string? StatusName { get; set; }

    public int? DisplayOrder { get; set; }

    public bool? IsActive { get; set; }

    public Guid? TenantId { get; set; }

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
