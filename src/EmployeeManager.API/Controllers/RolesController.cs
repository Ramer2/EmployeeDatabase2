using EmployeeManager.Services.Services.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManager.API.controllers;

[ApiController]
[Route("api/roles/[controller]")]
public class RolesController
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    [Route("/api/roles")]
    public async Task<IResult> GetAllRoles(CancellationToken cancellationToken)
    {
        try
        {
            var roles = await _roleService.GetAllRoles(cancellationToken);
            if (roles.Count == 0)
                return Results.NotFound();
            
            return Results.Ok(roles);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }
}