namespace EmployeeManager.Services.services.Tokens;

public interface ITokenService
{
    public string GenerateToken(string username, string role, string email);
}