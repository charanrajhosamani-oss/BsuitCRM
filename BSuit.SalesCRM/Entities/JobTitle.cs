using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class JobTitle
{
    public Guid JobTitleId { get; set; }

    public string JobTitleName { get; set; } = null!;

    public string? Description { get; set; }

    public int? DisplayOrder { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? TenantId { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<Lead> Leads { get; set; } = new List<Lead>();
}
