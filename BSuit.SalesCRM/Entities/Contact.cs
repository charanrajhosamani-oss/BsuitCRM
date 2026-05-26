using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class Contact
{
    public Guid ContactId { get; set; }

    public Guid? AccountId { get; set; }

    public Guid? SalutationId { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? PersonalEmail1 { get; set; }

    public string? PersonalEmail2 { get; set; }

    public string? Phone { get; set; }

    public string? MobilePhone { get; set; }

    public string? Department { get; set; }

    public string? Designation { get; set; }

    public string? SkpeId { get; set; }

    public string? TwitterURL { get; set; }

    public string? FacebookURL { get; set; }

    public Guid? TenantId { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public Guid? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }

    public virtual Account? Account { get; set; }

    public virtual SalutationMaster? Salutation { get; set; }
}
