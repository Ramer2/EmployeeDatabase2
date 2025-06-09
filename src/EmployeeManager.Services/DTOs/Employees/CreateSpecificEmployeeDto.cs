using System.ComponentModel.DataAnnotations;

namespace EmployeeManager.Services.dtos.employees;

public class CreateSpecificEmployeeDto
{
    [Required]
    public CreateSpecificPersonDto Person { get; set; } = null!;
    
    [Required]
    public decimal Salary { get; set; }
    
    [Required]
    public int PositionId { get; set; }
}