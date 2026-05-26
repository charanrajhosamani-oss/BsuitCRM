using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class CompanySize
{
    public Guid LeadCompanySizeId { get; set; }

    public string SizeName { get; set; } = null!;

    public int? MinEmployees { get; set; }

    public int? MaxEmployees { get; set; }

    public string? Description { get; set; }

    public int? DisplayOrder { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? TenantId { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<Lead> Leads { get; set; } = new List<Lead>();
}
