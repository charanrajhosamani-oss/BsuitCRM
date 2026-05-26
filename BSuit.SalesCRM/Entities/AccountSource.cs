using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class AccountSource
{
    public Guid AccountSourceId { get; set; }

    public string AccountSourceName { get; set; } = null!;

    public string? Description { get; set; }

    public int? DisplayOrder { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? TenantId { get; set; }
}
