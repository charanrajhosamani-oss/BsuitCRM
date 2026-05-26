using BSuit.HR.Data;
using Microsoft.EntityFrameworkCore;
using BSuit.Contracts.DTO.HR.Employee;

namespace BSuit.HR.Services.Employees
{
    public class EmployeeService : IEmployeeService
    {
        private readonly HRDbContext _db;

        public EmployeeService(HRDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync(Guid tenantId)
        {
            return await _db.Employees
                .Where(e => e.TenantId == tenantId)               
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    Name = $"{e.FirstName} {e.LastName}",
                    Email = e.Email,
                })
                .ToListAsync();
        }

        public async Task<EmployeeDto?> GetEmployeeByIdAsync(Guid tenantId, int employeeId)
        {
            var emp = await _db.Employees               
                .FirstOrDefaultAsync(e => e.TenantId == tenantId && e.Id == employeeId);

            if (emp == null) return null;

            return new EmployeeDto
            {
                Id = emp.Id,
                Name = $"{emp.FirstName} {emp.LastName}",
                Email = emp.Email,
            };
        }
    }
}