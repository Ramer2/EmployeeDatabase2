using EmployeeManager.Services.dtos.employees;

namespace EmployeeManager.Services.Services.Employees;

public interface IEmployeeService
{
    public Task<List<GetAllEmployeeDto>> GetAllEmployees(CancellationToken cancellationToken);
    
    public Task<GetSpecificEmployeeDto?> GetEmployeeById(int id, CancellationToken cancellationToken);
    
    public Task<bool> CreateEmployee(CreateSpecificEmployeeDto newEmployee, CancellationToken cancellationToken);
}