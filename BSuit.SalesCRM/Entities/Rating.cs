using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class Rating
{
    public Guid RatingId { get; set; }

    public string RatingName { get; set; } = null!;

    public string? Description { get; set; }

    public int? Score { get; set; }

    public int? DisplayOrder { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public Guid? TenantId { get; set; }

    public virtual ICollection<Lead> Leads { get; set; } = new List<Lead>();
}
