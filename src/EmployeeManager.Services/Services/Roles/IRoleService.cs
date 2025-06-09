using EmployeeManager.Services.dtos.accounts;

namespace EmployeeManager.Services.Services.Roles;

public interface IRoleService
{
    public Task<List<GetAllRolesDto>> GetAllRoles(CancellationToken cancellationToken);
}