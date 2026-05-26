
using BSuit.Core.Data;
using BSuit.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace BSuit.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBSuitsCore(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<CoreDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));         

            return services;
        }
    }
}