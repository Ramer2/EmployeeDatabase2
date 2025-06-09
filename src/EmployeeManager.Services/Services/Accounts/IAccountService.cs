using EmployeeManager.Services.dtos.accounts;

namespace EmployeeManager.Services.Services.Accounts;

public interface IAccountService
{
    public Task<List<GetAllAccountsDto>> GetAllAccounts(CancellationToken cancellationToken);
    
    public Task<GetSpecificAccountDto> GetSpecificAccount(int id, CancellationToken cancellationToken);
    
    public Task<bool> CreateAccount(CreateAccountDto createAccountDto, CancellationToken cancellationToken);

    public Task<bool> UpdateAccount(int id, UpdateAccountDto updateAccountDto, CancellationToken cancellationToken);
    
    public Task<bool> DeleteAccount(int accountId, CancellationToken cancellationToken);
    
    public Task<GetSpecificAccountDto> ViewAccount(string email, int id, CancellationToken cancellationToken);
    
    public Task<bool> UpdateUsersData(string email, int id, UpdateAccountDto updateDto, CancellationToken cancellationToken);
}