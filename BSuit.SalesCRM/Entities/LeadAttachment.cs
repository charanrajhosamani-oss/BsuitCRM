using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class LeadAttachment
{
    public Guid LeadAttachmentId { get; set; }

    public Guid? LeadId { get; set; }

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? TenantId { get; set; }

    public virtual Lead? Lead { get; set; }
}
