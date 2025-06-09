using EmployeeManager.Services.Services.Positions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManager.API.controllers;

[ApiController]
[Route("api/positions/[controller]")]
public class PositionsController
{
    private readonly IPositionService _positionService;

    public PositionsController(IPositionService positionService)
    {
        _positionService = positionService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    [Route("/api/positions")]
    public async Task<IResult> GetAllPositions(CancellationToken cancellationToken)
    {
        try
        {
            var positions = await _positionService.GetAllPositions(cancellationToken);
            if (positions.Count == 0)
                return Results.NotFound();
            
            return Results.Ok(positions);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }
}