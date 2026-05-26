using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BSuit.API.Infrastructure.Services
{
    public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
    {
        private const string APIKEYHEADER = "x-api-key";

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var configuration = context.HttpContext.RequestServices
                .GetRequiredService<IConfiguration>();

            if (!context.HttpContext.Request.Headers
                .TryGetValue(APIKEYHEADER, out var extractedApiKey))
            {
                context.Result = new UnauthorizedObjectResult(
                    "API Key missing");
                return;
            }

            var apiKey = configuration["ApiSecurity:ApiKey"];

            if (!apiKey.Equals(extractedApiKey))
            {
                context.Result = new UnauthorizedObjectResult(
                    "Invalid API Key");
                return;
            }

            await next();
        }
    }
}
