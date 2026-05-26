using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class Country
{
    public Guid CountryId { get; set; }

    public string CountryName { get; set; } = null!;

    public string? CountryCode { get; set; }

    public string? PhoneCode { get; set; }

    public string? CurrencyCode { get; set; }

    public Guid? RegionId { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? TenantId { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<Lead> Leads { get; set; } = new List<Lead>();

    public virtual Region? Region { get; set; }
}
