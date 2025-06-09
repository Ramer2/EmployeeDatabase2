using EmployeeManager.Services.context;
using EmployeeManager.Services.dtos.accounts;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManager.Services.Services.Roles;

public class RoleService : IRoleService
{
    private EmployeeDatabaseContext _context;

    public RoleService(EmployeeDatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<GetAllRolesDto>> GetAllRoles(CancellationToken cancellationToken)
    {
        try
        {
            var roles = await _context.Roles.ToListAsync();
            var rolesDtos = new List<GetAllRolesDto>();

            foreach (var role in roles)
            {
                rolesDtos.Add(new GetAllRolesDto
                {
                    Id = role.Id,
                    Name = role.Name
                });
            }

            return rolesDtos;
        }
        catch (Exception)
        {
            throw new ApplicationException("Error occured while getting all positions");
        }
    }
}