using BSuit.SalesCRM.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Models;

namespace BSuit.API.Infrastructure.Services
{
    public class LeadAssignmentBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public LeadAssignmentBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();

                var context = scope.ServiceProvider
                    .GetRequiredService<SalesCRMContext>();

                try
                {
                    // ============================================
                    // 🔹 Get Unassigned Leads
                    // ============================================

                    var newStageId = await context.LeadStages
     .Where(x => x.StageName == "New")
     .Select(x => x.LeadStageId)
     .FirstOrDefaultAsync(stoppingToken);

                    var leads = await context.Leads
                        .Where(x =>
                            x.IsActive == true &&
                            x.OwnerId == null &&
                            x.LeadStageId == newStageId)
                        .ToListAsync(stoppingToken);

                    if (leads.Any())
                    {
                        // ============================================
                        // 🔹 Load Country -> Region Mapping
                        // ============================================

                        var countryRegionMap = await context.Countries
                            .ToDictionaryAsync(
                                x => x.CountryId,
                                x => x.RegionId,
                                stoppingToken);

                        // ============================================
                        // 🔹 Load Active Configurations
                        // ============================================

                        var configs = await context.LeadConfigurations
                            .Where(x =>
                                x.IsActive == true &&
                                x.EffectiveFrom <= DateTime.Now)
                            .ToListAsync(stoppingToken);

                        // ============================================
                        // 🔹 Assign Leads
                        // ============================================

                        foreach (var lead in leads)
                        {
                            // Skip if country missing
                            if (lead.CountryId == null)
                                continue;

                            // ============================================
                            // 🔹 Get RegionId from Country
                            // ============================================

                            if (!countryRegionMap.TryGetValue(
                                    lead.CountryId.Value,
                                    out var regionId))
                            {
                                continue;
                            }

                            // ============================================
                            // 🔹 Get Lead Service Mapping
                            // ============================================

                            var leadServices = await context.LeadServiceMappings
                                .Where(x => x.LeadId == lead.LeadId)
                                .Select(x => x.ServiceId)
                                .ToListAsync(stoppingToken);

                            if (!leadServices.Any())
                                continue;

                            // ============================================
                            // 🔹 Find Matching Configuration
                            // ============================================

                            var config = configs.FirstOrDefault(c =>
                                c.RegionId == regionId &&
                                leadServices.Contains(c.ServiceId) &&
                                c.TenantId == lead.TenantId);

                            if (config != null)
                            {
                                // ============================================
                                // 🔹 Assign Sales Executive
                                // ============================================

                                lead.OwnerId = config.SalesExecutiveId;

                                lead.ModifiedOn = DateTime.Now;
                            }
                        }

                        // ============================================
                        // 🔹 Save Changes
                        // ============================================

                        //await context.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    // TODO:
                    // Add logging here

                    Console.WriteLine(ex.Message);
                }

                // ============================================
                // 🔹 Scheduler Delay
                // Runs every 5 minutes
                // ============================================

                await Task.Delay(
                    TimeSpan.FromMinutes(2),
                    stoppingToken);
            }
        }
    }
}
