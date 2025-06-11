using EmployeeManager.Services.Services.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManager.API.controllers;

[ApiController]
[Route("api/roles/[controller]")]
public class RolesController
{
    private readonly IRoleService _roleService;
    private readonly ILogger<RolesController> _logger;

    public RolesController(IRoleService roleService, ILogger<RolesController> logger)
    {
        _roleService = roleService;
        _logger = logger;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    [Route("/api/roles")]
    public async Task<IResult> GetAllRoles(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get all roles request.");
        
        try
        {
            var roles = await _roleService.GetAllRoles(cancellationToken);
            if (roles.Count == 0)
                return Results.NotFound();
            
            return Results.Ok(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Get all roles failed.\n{ex.Message}\n{ex.StackTrace}");
            return Results.Problem(ex.Message);
        }
    }
}