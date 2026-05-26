using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class ProjectMoudleEmployeemap
{
    public Guid ProjectMoudleEmployeemapId { get; set; }

    public Guid ProjectModuleId { get; set; }

    public Guid AssignedEmployeeId { get; set; }

    public long AssignedEmployeeHour { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? TenantId { get; set; }

    public virtual ProjectModule ProjectModule { get; set; } = null!;
}
