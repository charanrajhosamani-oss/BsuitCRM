using BSuit.SalesCRM.Data;
using BSuit.SalesCRM.Services;
using BSuit.SalesCRM.Services.ILeadService;
using BSuit.SalesCRM.Services.LeadService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace BSuit.SalesCRM
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBSuitsSalesCRM(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<SalesCRMContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));


            services.AddScoped<LeadService>();
            return services;
        }
    }
}