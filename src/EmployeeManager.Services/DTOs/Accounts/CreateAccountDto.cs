using System.ComponentModel.DataAnnotations;

namespace EmployeeManager.Services.dtos.accounts;

public class CreateAccountDto
{
    [Required]
    [StringLength(100)]
    [RegularExpression(@"^[^\d][\w\d_]{2,}$",
        ErrorMessage = "The username is not valid. Username shouldn’t start with numbers.")]
    public string Username { get; set; } = null!;

    [Required]
    [StringLength(100)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{12,}$",
        ErrorMessage = "Password is invalid. Password should have\nlength at least 12, and have at least" +
                       " one small letter, one capital letter, one number\nand one symbol.")]
    public string Password { get; set; } = null!;

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public int RoleId { get; set; }
}