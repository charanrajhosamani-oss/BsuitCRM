using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class OpportunityStage
{
    public Guid StageId { get; set; }

    public string? StageName { get; set; }

    public string? RoleId { get; set; }

    public int? ProbabilityPercentage { get; set; }

    public int? DisplayOrder { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? TenantId { get; set; }
}
