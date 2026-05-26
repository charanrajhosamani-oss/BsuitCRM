using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class ServiceUserRoleMap
{
    public long Id { get; set; }

    public Guid ServiceId { get; set; }

    public Guid UserId { get; set; }

    public Guid RoleId { get; set; }

    public DateTime AssignedOn { get; set; }
}
