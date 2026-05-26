using BSuit.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BSuit.Core
{
    /// <summary>
    /// EF CLI design-time factory for CoreDbContext
    /// </summary>
    public class CoreDbContextFactory
        : IDesignTimeDbContextFactory<CoreDbContext>
    {
        public CoreDbContext CreateDbContext(string[] args)
        {
            // Load configuration from API project
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../BSuit.API");

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<CoreDbContext>();

            optionsBuilder.UseSqlServer(
                config.GetConnectionString("DefaultConnection"));

            // ✅ Pass NULL for IUserContext (design-time only)
            return new CoreDbContext(optionsBuilder.Options, null);
        }
    }
}