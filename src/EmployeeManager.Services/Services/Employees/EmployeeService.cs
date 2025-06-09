using EmployeeManager.Models.models;
using EmployeeManager.Services.context;
using EmployeeManager.Services.dtos.employees;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManager.Services.Services.Employees;

public class EmployeeService : IEmployeeService
{
    private EmployeeDatabaseContext _context;

    public EmployeeService(EmployeeDatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<GetAllEmployeeDto>> GetAllEmployees(CancellationToken cancellationToken)
    {
        try
        {
            var employees = await _context.Employees
                .Include(p => p.Person)
                .ToListAsync();
            
            var employeeDtos = new List<GetAllEmployeeDto>();
        
            // mapping
            foreach (var employee in employees)
            {
                employeeDtos.Add(new GetAllEmployeeDto
                {
                    Id = employee.Id,
                    FullName = $"{employee.Person.FirstName} {employee.Person.MiddleName} {employee.Person.LastName}"
                });
            }

            return employeeDtos;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error while getting all employees", ex);
        }
    }

    public async Task<GetSpecificEmployeeDto?> GetEmployeeById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var employee = await _context.Employees
                .Include(p => p.Person)
                .Include(pos => pos.Position)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
            
            if (employee == null) return null;

            var personDto = new GetSpecificPersonDto
            {
                FirstName = employee.Person.FirstName,
                MiddleName = employee.Person.MiddleName,
                LastName = employee.Person.LastName,
                Email = employee.Person.Email,
                PassportNumber = employee.Person.PassportNumber,
                PhoneNumber = employee.Person.PhoneNumber
            };
        
            return new GetSpecificEmployeeDto
            {
                Person = personDto,
                Salary = employee.Salary,
                Position = employee.Position.Name,
                
                // I swear to everything thats Holy, I found this on accident lmao. YOU CAN DO THAT?!
                HireDate = $"{employee.HireDate:yyyy-MM-dd}"
            };
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error while getting employee by id", ex);
        }
    }

    public async Task<bool> CreateEmployee(CreateSpecificEmployeeDto newEmployee, CancellationToken cancellationToken)
    {
        try
        {
            var position = _context.Positions
                .FirstOrDefault(p => p.Id == newEmployee.PositionId);
            
            if (position == null)
                throw new ArgumentException("Invalid position");
            
            if (newEmployee.Salary < 0)
                throw new ArgumentException("Invalid salary (cannot be negative).");

            var person = new Person
            {
                PassportNumber = newEmployee.Person.PassportNumber,
                FirstName = newEmployee.Person.FirstName,
                MiddleName = newEmployee.Person.MiddleName,
                LastName = newEmployee.Person.LastName,
                PhoneNumber = newEmployee.Person.PhoneNumber,
                Email = newEmployee.Person.Email,
            };

            var employee = new Employee
            {
                Salary = newEmployee.Salary,
                Person = person,
                Position = position,
                HireDate = DateTime.Now,
            };
            
            await _context.Employees.AddAsync(employee, cancellationToken);
            _context.SaveChanges();
            return true;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error while creating new employee", ex);
        }
    }
}