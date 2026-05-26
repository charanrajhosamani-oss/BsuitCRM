using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class Referral
{
    public Guid ReferralId { get; set; }

    public Guid ReferrerAccountId { get; set; }

    public Guid? ReferredLeadId { get; set; }

    public string? ReferredName { get; set; }

    public string? ReferredPhone { get; set; }

    public string? ReferredEmail { get; set; }

    public bool? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Account ReferrerAccount { get; set; } = null!;
}
