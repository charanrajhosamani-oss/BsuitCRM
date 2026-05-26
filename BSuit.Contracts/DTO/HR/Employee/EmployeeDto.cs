
namespace BSuit.Contracts.DTO.HR.Employee
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    // Contract interface
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync(Guid tenantId);
        Task<EmployeeDto?> GetEmployeeByIdAsync(Guid tenantId, int employeeId);
    }
}
