using BSuit.API.Infrastructure.Constants;
using BSuit.Contracts.Services;
using DocumentFormat.OpenXml.InkML;
using System.Security.Claims;

namespace BSuit.API.Infrastructure.Services;


public class HttpUserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public string? UserId =>
        User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public string? UserName =>
        User?.Identity?.Name;

    public Guid? TenantId
    {
        get
        {
            var tenantClaim = User?.FindFirst(PARAMS.TENANT_ID)?.Value;  
            if (Guid.TryParse(tenantClaim, out var tenantId))
                return tenantId;

            return null;
        }
    }

    public Guid? GUID_USERID
    {
        get
        {
            var guidUserIdClaim = User?.FindFirst(PARAMS.GUID_USER_ID)?.Value;
            if (Guid.TryParse(guidUserIdClaim, out var GUIDUSERID))
                return GUIDUSERID;

            return null;
        }
    }

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated ?? false;


    public bool IsSuperAdmin =>
     User?.IsInRole(nameof(BSuit.Identity.AppRoles.SUPERADMIN)) ?? false;


    public bool IsInRole(string role) => User?.IsInRole(role) ?? false;

    public List<Guid> RoleIds
    {
        get
        {
            var roleClaims = User?
                .FindAll("RoleId")
                .Select(x => x.Value)
                .ToList();

            if (roleClaims == null || !roleClaims.Any())
                return new List<Guid>();

            return roleClaims
                .Where(x => Guid.TryParse(x, out _))
                .Select(Guid.Parse)
                .ToList();
        }
    }

}