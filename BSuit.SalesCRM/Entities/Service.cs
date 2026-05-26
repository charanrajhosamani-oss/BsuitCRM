using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class Service
{
    public Guid ServiceId { get; set; }

    public Guid? ParentServiceId { get; set; }

    public string ServiceName { get; set; } = null!;

    public string? Description { get; set; }

    public int? DisplayOrder { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public Guid? TenantId { get; set; }

    public virtual ICollection<LeadConfiguration> LeadConfigurations { get; set; } = new List<LeadConfiguration>();

    public virtual ICollection<LeadServiceMapping> LeadServiceMappingServices { get; set; } = new List<LeadServiceMapping>();

    public virtual ICollection<LeadServiceMapping> LeadServiceMappingSubServices { get; set; } = new List<LeadServiceMapping>();

    public virtual ICollection<ServicePricing> ServicePricings { get; set; } = new List<ServicePricing>();
}
