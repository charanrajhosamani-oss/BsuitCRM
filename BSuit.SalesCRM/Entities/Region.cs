using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class Region
{
    public Guid RegionId { get; set; }

    public string RegionName { get; set; } = null!;

    public string? RegionCode { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public Guid? TenantId { get; set; }

    public virtual ICollection<Country> Countries { get; set; } = new List<Country>();

    public virtual ICollection<LeadConfiguration> LeadConfigurations { get; set; } = new List<LeadConfiguration>();
}
