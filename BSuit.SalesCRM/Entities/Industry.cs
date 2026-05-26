using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class Industry
{
    public Guid IndustryId { get; set; }

    public Guid? ParentIndustryId { get; set; }

    public string IndustryName { get; set; } = null!;

    public string? Description { get; set; }

    public int? DisplayOrder { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? TenantId { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<Industry> InverseParentIndustry { get; set; } = new List<Industry>();

    public virtual ICollection<Lead> Leads { get; set; } = new List<Lead>();

    public virtual Industry? ParentIndustry { get; set; }
}
