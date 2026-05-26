using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class CustomerCategory
{
    public Guid CustomerCategoryId { get; set; }

    public string CustomerCategory1 { get; set; } = null!;

    public string? Description { get; set; }

    public int? DisplayOrder { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? TenantId { get; set; }
}
