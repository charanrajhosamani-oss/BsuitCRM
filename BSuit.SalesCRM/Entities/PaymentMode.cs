using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class PaymentMode
{
    public Guid ModeOfPaymentId { get; set; }

    public string PaymentModeName { get; set; } = null!;

    public string? PaymentModeCode { get; set; }

    public string? Description { get; set; }

    public int? DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public Guid? ModifiedBy { get; set; }
}
