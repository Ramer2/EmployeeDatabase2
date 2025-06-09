namespace EmployeeManager.Services.dtos.employees;

public class GetSpecificEmployeeDto
{
    public GetSpecificPersonDto Person { get; set; }
    public decimal Salary { get; set; }
    
    public string Position { get; set; }
    public string HireDate { get; set; }
}