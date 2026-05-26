using BSuit.Contracts.DTO.HR.Employee;
using BSuit.SalesCRM.Entities;
using BSuit.SalesCRM.VM.Lead;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSuit.SalesCRM.Services.ILeadService
{
    public interface ILeadService
    {
        Task<IEnumerable<Lead>> GetAllLeadAsync(Guid tenantId);
        Task<Lead?> GetLeadByIdAsync(Guid tenantId, Guid leadId);

        Task SaveLeadConfigurationAsync(List<LeadConfiguration> data);

    }
}
