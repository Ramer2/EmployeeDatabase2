using EmployeeManager.Services.dtos.employees;

namespace EmployeeManager.Services.Services.Positions;

public interface IPositionService
{
    public Task<List<GetAllPositionsDto>> GetAllPositions(CancellationToken cancellationToken);
}