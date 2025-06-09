using EmployeeManager.Models.models;
using EmployeeManager.Services.context;
using EmployeeManager.Services.dtos;
using EmployeeManager.Services.dtos.auth;
using EmployeeManager.Services.services.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManager.API.controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly EmployeeDatabaseContext _context;
    private readonly ITokenService _tokenService;
    private readonly PasswordHasher<Account> _passwordHasher = new();

    public AuthController(EmployeeDatabaseContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost]
    [Route("/api/auth")]
    public async Task<IResult> Auth(LoginUserDto user, CancellationToken cancellationToken)
    {
        var foundAccount = await _context.Accounts
            .Include(a => a.Role)
            .Include(acc => acc.Employee)
            .ThenInclude(e => e.Person)
            .FirstOrDefaultAsync(a => string.Equals(a.Username, user.Login), cancellationToken);
        
        if (foundAccount == null)
            return Results.Unauthorized();

        var verificationResult = _passwordHasher.VerifyHashedPassword(foundAccount, foundAccount.Password, user.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
            return Results.Unauthorized();

        var token = new TokenDto
        {
            AccessToken = _tokenService.GenerateToken(
                foundAccount.Password, 
                foundAccount.Role.Name, 
                foundAccount.Employee.Person.Email)
        };
        return Results.Ok(token);
    }
}