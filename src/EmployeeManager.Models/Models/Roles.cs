namespace EmployeeManager.Models.models;

using System.ComponentModel.DataAnnotations;

public class Roles
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;
    
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
