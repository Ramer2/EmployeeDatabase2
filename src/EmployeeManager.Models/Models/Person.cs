using System.ComponentModel.DataAnnotations;

namespace EmployeeManager.Models.models;

public partial class Person
{
    public int Id { get; set; }

    public string PassportNumber { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    public string Email { get; set; } = null!;

    [RegularExpression(@"^\+?[0-9\s\-]{7,15}$")]
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
