namespace BSuit.Contracts.Services;
public class UserContext : IUserContext
{
    public string? UserId => "SYSTEM";
    public string? UserName => "System Seeder";
    // ❌ REMOVE RoleId completely

    // ✅ ADD multi-role support (empty for system context)
    public List<Guid> RoleIds => new();

    public Guid? TenantId => null;
    public bool IsAuthenticated => false;
    public bool IsSuperAdmin => false;

    public Guid? GUID_USERID => null;
}