using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class PaymentType
{
    public Guid PaymentTypeId { get; set; }

    public string PaymentTypeName { get; set; } = null!;

    public string? Description { get; set; }

    public int? DisplayOrder { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? TenantId { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
