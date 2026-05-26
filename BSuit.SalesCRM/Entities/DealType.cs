using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class DealType
{
    public Guid DealTypeId { get; set; }

    public string DealTypeName { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? TenantId { get; set; }
}
