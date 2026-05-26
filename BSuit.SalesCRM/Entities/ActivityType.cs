using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class ActivityType
{
    public Guid ActivityTypeId { get; set; }

    public string ActivityTypeName { get; set; } = null!;

    public string? ActivityCode { get; set; }

    public int? DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }
}
