using BSuit.Contracts.DTO.HR.Employee;
using BSuit.HR.Entities;


namespace BSuit.HR.Services.IEmployees
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllAsync(Guid tenantId);
        Task<EmployeeDto?> GetEmployeeByIdAsync(Guid tenantId, int employeeId);
        //Task<EmployeeDto> CreateAsync(Employee employee);
    }
}
