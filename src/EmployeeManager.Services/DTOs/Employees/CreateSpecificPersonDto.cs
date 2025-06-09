using System.ComponentModel.DataAnnotations;

namespace EmployeeManager.Services.dtos.employees;

public class CreateSpecificPersonDto
{
    [Required]
    [StringLength(10)]
    public string PassportNumber { get; set; } = null!;
    
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = null!;
    
    [StringLength(50)]
    public string? MiddleName { get; set; }
    
    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = null!;
    
    [Required]
    [StringLength(50)]
    [RegularExpression(@"^\+\d{9,15}$", 
        ErrorMessage = "Phone number is invalid. It must start with '+' and contain 9 to 15 digits with no spaces or symbols.")]
    public string PhoneNumber { get; set; } = null!;
    
    [Required]
    [StringLength(50)]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        ErrorMessage = "The email address is not valid.")]
    public string Email { get; set; } = null!;
    
}