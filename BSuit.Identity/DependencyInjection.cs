using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BSuit.Identity.Data;
using BSuit.Identity.Models;
namespace BSuit.Identity
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBSuitsIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // ✅ DbContext
            services.AddDbContext<IdentityDbContext>(options => options.UseSqlServer(connectionString));

            // ✅ Identity Configuration
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // 🔐 Password
                options.Password.RequiredLength = 6; 
                options.Password.RequireDigit = true;
                // 🔒 Lockout
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); 
                options.Lockout.MaxFailedAccessAttempts = 5; 
                options.Lockout.AllowedForNewUsers = true;
                // 👤 User
                options.User.RequireUniqueEmail = true;
                // 🔑 Sign-in
                options.SignIn.RequireConfirmedAccount = false;
                // change if needed
            }).AddEntityFrameworkStores<IdentityDbContext>().AddDefaultTokenProviders(); 
            
            return services;
        }
    }
}