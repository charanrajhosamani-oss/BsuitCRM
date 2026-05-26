using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class EmailRecipient
{
    public Guid RecipientId { get; set; }

    public Guid EmailId { get; set; }

    public string Address { get; set; } = null!;

    public string Type { get; set; } = null!;

    public virtual EmailMessage Email { get; set; } = null!;
}
