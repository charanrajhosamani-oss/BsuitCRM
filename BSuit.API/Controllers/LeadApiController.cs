
using BSuit.API.Infrastructure.Services;
using BSuit.API.Models;
using BSuit.Core.Data;
using BSuit.SalesCRM.Data;
using BSuit.SalesCRM.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.RegularExpressions;


namespace BSuit.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadApiController : ControllerBase
    {
        private readonly SalesCRMContext _context;
        private readonly ILogger<LeadApiController> _logger;

        public LeadApiController(
           SalesCRMContext context,
           ILogger<LeadApiController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [ApiKeyAuth]
        [HttpPost("CreateLead")]
        public async Task<IActionResult> CreateLead(
            [FromBody] LeadRequestDto model)
        {
            if (model == null)
            {
                _logger.LogWarning("Lead API received null request payload");
                return BadRequest("Invalid request");
            }

            _logger.LogInformation(
                "Lead API Request Received: {Payload}",
                JsonSerializer.Serialize(model));

            using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var leadId = Guid.NewGuid();

                Guid? CountryId = _context.Countries.Where(c => c.CountryName.Contains(model.CountryName))?
                    .FirstOrDefault()?.CountryId;
                if (CountryId.HasValue == false || CountryId.Value == Guid.Empty)
                    CountryId = Guid.Parse("63B103E0-C926-F111-8435-D404E6DFC771");//US

                Guid? IndustryId = _context.Industries.Where(c => c.IndustryName.Contains(model.IndustryName))?
                    .FirstOrDefault()?.IndustryId;
                if (IndustryId.HasValue == false || IndustryId.Value == Guid.Empty)
                    IndustryId = Guid.Parse("2b87a25c-8e47-f111-8437-d404e6dfc771");//Others

                Guid? LeadSourceId = _context.LeadSources.Where(c => c.SourceName == model.LeadSourceName)?
                    .FirstOrDefault()?.LeadSourceId;
                if (LeadSourceId.HasValue == false || LeadSourceId.Value == Guid.Empty)
                    LeadSourceId = Guid.Parse("3EF9B4B6-9E47-F111-8437-D404E6DFC771");//Others
                
                var lead = new Lead
                {
                    LeadId = leadId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Phone = model.Phone,
                    CompanyName = model.CompanyName,
                    City = model.City,
                    Website = model.Website,
                    RequirementDetails = model.RequirementDetails,
                    CountryId = CountryId,
                    IndustryId = IndustryId,
                    LeadSourceId = LeadSourceId,
                    CreatedOn = DateTime.UtcNow,
                    IsActive = true,
                    //OwnerId = 
                };

                _context.Leads.Add(lead);
                await _context.SaveChangesAsync();

                if (model.Services != null && model.Services.Any())
                {
                    foreach (var item in model.Services)
                    {
                        var serviceId = _context.Services.Where(s => s.ServiceName == item.SubServiceName)?
                            .FirstOrDefault()?.ServiceId;

                        if (serviceId.HasValue == false || serviceId == Guid.Empty)
                        {
                            serviceId = _context.Services.First().ServiceId;
                            item.Notes = $"IGNORE Service name here - {item.Notes}";
                        }

                        var serviceMapping =
                            new LeadServiceMapping
                            {
                                LeadServiceId = Guid.NewGuid(),
                                LeadId = leadId,
                                ServiceId = serviceId.HasValue == true ? serviceId.Value : Guid.Empty,
                                SubServiceId = serviceId.HasValue == true ? serviceId.Value : Guid.Empty,
                                Notes = item.Notes,
                                CreatedOn = DateTime.UtcNow
                            };

                        _context.LeadServiceMappings
                            .Add(serviceMapping);
                    }

                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                return Ok(new
                {
                    success = true,
                    message = "Lead created successfully",
                    leadId = leadId
                });
            }
            catch (Exception ex)
            {
                _logger.LogInformation(
                "Lead API Exception: {Payload}",
                JsonSerializer.Serialize(ex.Message));

                await transaction.RollbackAsync();

                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


    }
}