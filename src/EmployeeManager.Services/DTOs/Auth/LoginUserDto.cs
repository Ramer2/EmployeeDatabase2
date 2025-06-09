using System.ComponentModel.DataAnnotations;

namespace EmployeeManager.Services.dtos.auth;

public class LoginUserDto
{
    [Required]
    [RegularExpression(@"^[^\d][\w\d_]{2,}$")]
    public string Login { get; set; }
    
    [Required]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{12,}$")]
    public string Password { get; set; }
}