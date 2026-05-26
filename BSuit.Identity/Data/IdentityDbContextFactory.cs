using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using BSuit.Identity.Data;

namespace BSuit.Identity
{
    public class IdentityDbContextFactory
        : IDesignTimeDbContextFactory<IdentityDbContext>
    {
        public IdentityDbContext CreateDbContext(string[] args)
        {
            // Load configuration from API project
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../BSuit.API");

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();

            optionsBuilder.UseSqlServer(
                config.GetConnectionString("DefaultConnection"));

            return new IdentityDbContext(optionsBuilder.Options, null);
        }
    }
}