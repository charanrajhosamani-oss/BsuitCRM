using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class AccountCategorization
{
    public Guid AccountCategorizationId { get; set; }

    public string AccountCategorization1 { get; set; } = null!;

    public string? Description { get; set; }

    public int? DisplayOrder { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? TenantId { get; set; }
}
