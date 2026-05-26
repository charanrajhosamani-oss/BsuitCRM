using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class Opportunity
{
    public Guid OpportunityId { get; set; }

    public string? OpportunityName { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? TenantId { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public virtual ICollection<OpportunityModule> OpportunityModules { get; set; } = new List<OpportunityModule>();

    public virtual ICollection<ProjectModule> ProjectModules { get; set; } = new List<ProjectModule>();
}
