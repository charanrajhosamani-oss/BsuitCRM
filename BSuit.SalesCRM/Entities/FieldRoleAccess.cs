using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class FieldRoleAccess
{
    public Guid AccessId { get; set; }

    public string? RoleId { get; set; }

    public Guid? ModuleId { get; set; }

    public Guid? FormId { get; set; }

    public Guid? FieldId { get; set; }

    public bool? CanView { get; set; }

    public bool? CanEdit { get; set; }

    public bool? IsHidden { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Field? Field { get; set; }

    public virtual Form? Form { get; set; }

    public virtual Module? Module { get; set; }
}
