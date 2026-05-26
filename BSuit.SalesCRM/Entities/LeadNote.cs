using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class LeadNote
{
    public Guid NoteId { get; set; }

    public Guid? LeadId { get; set; }

    public string? Notes { get; set; }

    public bool? IsActive { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? TenantId { get; set; }

    public virtual Lead? Lead { get; set; }
}
