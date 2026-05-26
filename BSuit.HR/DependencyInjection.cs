

using BSuit.Contracts.DTO.HR.Employee;
using BSuit.HR.Data;
using BSuit.HR.Services.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace BSuit.HR
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBSuitsHR(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // 1️. Register DbContext for HR module
            services.AddDbContext<HRDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));


            // 2️. Register HR module services that implement Contracts
            services.AddScoped<IEmployeeService, EmployeeService>();

            return services;
        }
    }
}