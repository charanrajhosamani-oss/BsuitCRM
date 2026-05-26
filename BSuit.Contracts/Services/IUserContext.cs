namespace BSuit.Contracts.Services
{
    public interface IUserContext
    {
        Guid? GUID_USERID { get; }
        string? UserId { get; }
        string? UserName { get; }
        Guid? TenantId { get; }
        bool IsAuthenticated { get; }
        bool IsSuperAdmin { get; }
        // ✅ NEW (multi-role support)
        List<Guid> RoleIds { get; }

        // optional (VERY useful for UI/authorization)
    }
}
