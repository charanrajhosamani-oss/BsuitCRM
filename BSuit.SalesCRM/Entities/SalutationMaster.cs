using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class SalutationMaster
{
    public Guid SalutationId { get; set; }

    public string SalutationName { get; set; } = null!;

    public string? Description { get; set; }

    public int? DisplayOrder { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? TenantId { get; set; }

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}
