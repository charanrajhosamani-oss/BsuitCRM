using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class LeadRejection
{
    public Guid LeadRejectionId { get; set; }

    public Guid LeadId { get; set; }

    public Guid? RejectReasonId { get; set; }

    public string? RejectionRemarks { get; set; }

    public bool? IsApproved { get; set; }

    public Guid? ApprovedBy { get; set; }

    public DateTime? ApprovedDate { get; set; }

    public Guid? RejectedBy { get; set; }

    public DateTime? RejectedDate { get; set; }

    public bool? IsActive { get; set; }

    public Guid? PrevStageId { get; set; }
}
