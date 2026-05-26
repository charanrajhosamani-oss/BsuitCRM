using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class Module
{
    public Guid ModuleId { get; set; }

    public string ModuleName { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime? CreatedOn { get; set; }

    public virtual ICollection<FieldRoleAccess> FieldRoleAccesses { get; set; } = new List<FieldRoleAccess>();

    public virtual ICollection<Form> Forms { get; set; } = new List<Form>();
}
