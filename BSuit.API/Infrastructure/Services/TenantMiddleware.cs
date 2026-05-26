using BSuit.API.Areas.Admin.Models;
using BSuit.API.Infrastructure.Constants;
using BSuit.Contracts.Services;
using Microsoft.EntityFrameworkCore;

namespace BSuit.API.Infrastructure.Services
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TenantMiddleware> _logger;

        public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(
            HttpContext context,
            Core.Data.CoreDbContext dbContext,
            IUserContext userContext)
        {
            var path = context.Request.Path.Value;
            if (path.StartsWith("/css") ||
                path.StartsWith("/js") ||
                path.StartsWith("/lib") ||
                path.EndsWith(".js") ||
                path.StartsWith("/images"))
            {
                await _next(context);
                return;
            }

            if (dbContext.Database.CanConnect() == false)
            {
                await _next(context);
                return;
            }
            // =================================================================
            // 1. Logged-in is Tenant User
            // =================================================================
            if (userContext.IsAuthenticated && 
                userContext.TenantId.HasValue && 
                userContext.GUID_USERID != Guid.Empty &&
                userContext.IsSuperAdmin == false)
            {
                var tenantId = userContext.TenantId;
                context.Items[PARAMS.TENANT_ID] = tenantId;

                // ===============================================================================
                // 2. Subscription Validation
                // ===============================================================================
                var tenantSub = await dbContext.TenantSubscriptions
                    .AsNoTracking()
                    .Where(x => x.TenantId == tenantId && x.IsActive)
                    .OrderByDescending(x => x.EndDate)
                    .FirstOrDefaultAsync();

                if (tenantSub == null || tenantSub.EndDate < DateTime.UtcNow)
                {
                    _logger.LogWarning("Subscription expired for TenantId: {TenantId}", tenantId);
                    context.Response.Redirect("/SubscriptionExpired");
                    return;
                }


                var module = dbContext.Modules.First(x => x.Code == MODULECODE.SALESCRM);
                if (module != null)
                {
                    var hasAccess = dbContext.TenantSubscriptionModules.Any(x =>
                        x.TenantSubscription.TenantId == tenantId &&
                        x.ModuleId == module.Id &&
                        x.IsActive);

                    if (!hasAccess)
                    {
                        context.Response.Redirect("/NoAccess");
                        return;
                    }
                }

            }


            await _next(context);
        }
    }
}