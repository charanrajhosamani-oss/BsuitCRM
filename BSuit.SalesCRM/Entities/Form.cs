using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class Form
{
    public Guid FormId { get; set; }

    public Guid? ModuleId { get; set; }

    public string? FormName { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<FieldRoleAccess> FieldRoleAccesses { get; set; } = new List<FieldRoleAccess>();

    public virtual ICollection<Field> Fields { get; set; } = new List<Field>();

    public virtual Module? Module { get; set; }
}
