using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class NDASignature
{
    public Guid NDAID { get; set; }

    public Guid? LeadId { get; set; }

    public Guid? AccountId { get; set; }

    public string? NDAVersionNumber { get; set; }

    public string? NDANumber { get; set; }

    public bool? AcceptNonDisclosureAgreement { get; set; }

    public bool? AcceptScannedDocumentsNDA { get; set; }

    public string? NDAFormat { get; set; }

    public string? NDASpecialClauseForOrg { get; set; }

    public string? NDASpecialClauseForInternal { get; set; }

    public DateOnly? NDAStartDate { get; set; }

    public DateOnly? NDAEndDate { get; set; }

    public DateOnly? NDARenewalDate { get; set; }

    public string? NDACustomerName { get; set; }

    public string? NDALink { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public Guid? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? ExecutedDate { get; set; }
}
