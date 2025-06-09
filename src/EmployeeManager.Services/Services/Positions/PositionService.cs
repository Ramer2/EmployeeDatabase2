using EmployeeManager.Services.context;
using EmployeeManager.Services.dtos.employees;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManager.Services.Services.Positions;

public class PositionService : IPositionService
{
    private EmployeeDatabaseContext _context;

    public PositionService(EmployeeDatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<GetAllPositionsDto>> GetAllPositions(CancellationToken cancellationToken)
    {
        try
        {
            var positions = await _context.Positions.ToListAsync(cancellationToken);
            var positionsDtos = new List<GetAllPositionsDto>();

            foreach (var position in positions)
            {
                positionsDtos.Add(new GetAllPositionsDto
                {
                    
                    Id = position.Id,
                    Name = position.Name
                });
            }
            
            return positionsDtos;
        } 
        catch (Exception)
        {
            throw new ApplicationException("Error getting all positions.");
        }
    }
}