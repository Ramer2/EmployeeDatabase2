namespace EmployeeManager.Services.dtos.accounts;

public class GetSpecificAccountDto
{
    public string Username { get; set; } = null!;
    
    public string Role { get; set; } = null!;
}