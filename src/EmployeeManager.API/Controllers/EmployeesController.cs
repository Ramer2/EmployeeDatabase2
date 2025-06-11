using EmployeeManager.Services.dtos.employees;
using EmployeeManager.Services.Services.Employees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManager.API.controllers;

[ApiController]
[Route("api/employees/[controller]")]
public class EmployeesController : ControllerBase
{
    private IEmployeeService _employeeService;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(IEmployeeService employeeService, ILogger<EmployeesController> logger)
    {
        _employeeService = employeeService;
        _logger = logger;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    [Route("/api/employees")]
    public async Task<IResult> GetAllEmployees(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get all employees request.");
        
        try
        {
            var employees = await _employeeService.GetAllEmployees(cancellationToken);
            if (employees.Count == 0) return Results.NotFound("No employees found");
            return Results.Ok(employees);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Get all employees failed.\n{ex.Message}\n{ex.StackTrace}");
            return Results.Problem(ex.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    [Route("/api/employees/{id}")]
    public async Task<IResult> GetEmployeeById(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get employee by id request.");
        
        if (id < 0) return Results.BadRequest("Invalid id");
        
        try
        {
            var employee = await _employeeService.GetEmployeeById(id, cancellationToken);
            if (employee == null) return Results.NotFound("Employee not found");
            return Results.Ok(employee);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Get employee by id failed.\n{ex.Message}\n{ex.StackTrace}");
            return Results.Problem(ex.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [Route("/api/employees")]
    public async Task<IResult> CreateEmployee(CreateSpecificEmployeeDto employee, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create employee request.");
        
        if (!ModelState.IsValid)
            return Results.BadRequest(ModelState);
        
        try
        {
            await _employeeService.CreateEmployee(employee, cancellationToken);
            return Results.Created();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning($"Create employee. {ex.Message}");
            return Results.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Create employee failed.\n{ex.Message}\n{ex.StackTrace}");
            return Results.Problem(ex.Message);
        }
    }
}