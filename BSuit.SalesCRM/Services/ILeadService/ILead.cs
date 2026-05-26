using BSuit.Contracts.DTO.HR.Employee;
using BSuit.SalesCRM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSuit.SalesCRM.Services.ILeadService
{
    internal interface ILead
    {
        Task<IEnumerable<Lead>> GetAllLeadAsync(Guid tenantId);
        Task<Lead?> GetLeadByIdAsync(Guid tenantId, Guid leadId);
    }
}
