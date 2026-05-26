using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class LeadJobTitle
{
    public Guid LeadJobTitleId { get; set; }

    public string JobTitleName { get; set; } = null!;

    public string? Description { get; set; }

    public int? DisplayOrder { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }
}
