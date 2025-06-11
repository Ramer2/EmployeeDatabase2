using EmployeeManager.Services.Services.Employees;
using EmployeeManager.Services.Services.Positions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManager.API.controllers;

[ApiController]
[Route("api/positions/[controller]")]
public class PositionsController
{
    private readonly IPositionService _positionService;
    private readonly ILogger<PositionsController> _logger;

    public PositionsController(IPositionService positionService, ILogger<PositionsController> logger)
    {
        _positionService = positionService;
        _logger = logger;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    [Route("/api/positions")]
    public async Task<IResult> GetAllPositions(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get all positions request.");
        
        try
        {
            var positions = await _positionService.GetAllPositions(cancellationToken);
            if (positions.Count == 0)
                return Results.NotFound();
            
            return Results.Ok(positions);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Get all positions failed.\n{ex.Message}\n{ex.StackTrace}");
            return Results.Problem(ex.Message);
        }
    }
}