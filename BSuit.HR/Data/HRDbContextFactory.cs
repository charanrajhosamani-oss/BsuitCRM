using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;


#nullable disable
namespace BSuit.HR.Data
{
    public class HRDbContextFactory
        : IDesignTimeDbContextFactory<HRDbContext>
    {
        public HRDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../BSuit.API");

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();


            //Detect encrypted value
            var connectionString = config.GetConnectionString("DefaultConnection");
           

            var optionsBuilder = new DbContextOptionsBuilder<HRDbContext>();
            optionsBuilder.UseSqlServer(connectionString);


            return new HRDbContext(optionsBuilder.Options);
        }
    }
}