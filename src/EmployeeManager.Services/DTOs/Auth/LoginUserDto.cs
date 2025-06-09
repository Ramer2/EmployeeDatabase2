using System.ComponentModel.DataAnnotations;

namespace EmployeeManager.Services.dtos.auth;

public class LoginUserDto
{
    [Required]
    public string Login { get; set; }
    
    [Required]
    public string Password { get; set; }
}