using System;
using System.Collections.Generic;

namespace BSuit.HR.Entities;

public partial class Employee
{
    public int Id { get; set; }

    public Guid? TenantId { get; set; }

    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public string Email { get; set; } = null!;
}
