using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class Field
{
    public Guid FieldId { get; set; }

    public Guid? FormId { get; set; }

    public string? FieldName { get; set; }

    public string? FieldLabel { get; set; }

    public string? DataType { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<FieldRoleAccess> FieldRoleAccesses { get; set; } = new List<FieldRoleAccess>();

    public virtual Form? Form { get; set; }
}
