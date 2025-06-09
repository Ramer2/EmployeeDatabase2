using System.ComponentModel.DataAnnotations;

namespace EmployeeManager.Models.models;

public class Account
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [RegularExpression(@"^[^\d][\w\d_]{2,}$")]
    public string Username { get; set; } = null!;

    [Required]
    [StringLength(100)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{12,}$")]
    public string Password { get; set; } = null!;

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public int RoleId { get; set; }

    public virtual Role Role { get; set; } = null!;
    
    public virtual Employee Employee { get; set; } = null!;
}