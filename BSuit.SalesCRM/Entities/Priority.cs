using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class Priority
{
    public Guid PriorityId { get; set; }

    public string PriorityName { get; set; } = null!;

    public string? Description { get; set; }

    public string? ColorCode { get; set; }

    public int? DisplayOrder { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public Guid? TenantId { get; set; }

    public virtual ICollection<Lead> Leads { get; set; } = new List<Lead>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
