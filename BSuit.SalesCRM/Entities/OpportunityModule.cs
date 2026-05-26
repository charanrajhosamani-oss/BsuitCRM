using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class OpportunityModule
{
    public Guid OpportunityModuleId { get; set; }

    public Guid? OpportunityId { get; set; }

    public Guid? ServiceId { get; set; }

    public string? ModuleName { get; set; }

    public string? Description { get; set; }

    public decimal? QCHours { get; set; }

    public decimal? EstimatedHours { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? TenantId { get; set; }

    public virtual Opportunity? Opportunity { get; set; }
}
