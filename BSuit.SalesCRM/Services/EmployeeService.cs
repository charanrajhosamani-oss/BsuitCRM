
using BSuit.Contracts.DTO.HR.Employee;

namespace BSuit.SalesCRM.Services
{
    public class EmployeeService
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeService(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task LinkEmployeeToEmployee(Guid tenantId, int employeeId)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(tenantId, employeeId);

            if (employee == null)
                throw new Exception("Employee not found");

            // Now SalesCRM can use employee info for business logic
            Console.WriteLine($"Assigning Employee {employee.Name} to a Employee");
        }
    }
}
