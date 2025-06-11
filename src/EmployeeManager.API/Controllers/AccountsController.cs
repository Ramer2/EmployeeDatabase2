using System.Security.Claims;
using EmployeeManager.Services.dtos.accounts;
using EmployeeManager.Services.Services.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManager.API.controllers;

[Route("api/accounts/[controller]")]
[ApiController]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ILogger<AccountsController> _logger;
    
    public AccountsController(IAccountService accountService, ILogger<AccountsController> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    [Route("/api/accounts")]
    public async Task<IResult> GetAllAccounts(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get all accounts request.");
        
        try
        {
            var accounts = await _accountService.GetAllAccounts(cancellationToken);
            if (accounts.Count == 0)
                return Results.NotFound();

            return Results.Ok(accounts);   
        }
        catch (Exception ex)
        {
            _logger.LogError($"Get all accounts failed.\n{ex.Message}\n{ex.StackTrace}");
            return Results.Problem(ex.Message);
        }
    }

    [Authorize(Roles = "Admin,User")]
    [HttpGet]
    [Route("/api/accounts/{id}")]
    public async Task<IResult> GetAccountById(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get account by id request.");
        
        try
        {
            if (User.IsInRole("Admin")) // Admin
            {
                // different method, because Admin may be able to see different data than the User
                // + no checks needed
                var account = await _accountService.GetSpecificAccount(id, cancellationToken);
                return Results.Ok(account);
            }
            else // User - check id
            {
                if (User.FindFirst(ClaimTypes.Email) == null)
                    return Results.Problem("Invalid credentials");

                // different method, because User may be able to see different data than the Admin
                var res = await _accountService
                    .ViewAccount(User.FindFirst(ClaimTypes.Email)!.Value, id, cancellationToken);
                
                return Results.Ok(res);
            }
        }
        catch (AccessViolationException)
        {
            _logger.LogWarning($"Access violation: request for get account by id {id} was denied.");
            return Results.Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning($"No data for account with id {id}.");
            return Results.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Get account by id failed.\n{ex.Message}\n{ex.StackTrace}");
            return Results.Problem(ex.Message);
        }
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [Route("/api/accounts")]
    public async Task<IResult> CreateAccount([FromBody] CreateAccountDto newAccount, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create account request.");
        
        if (!ModelState.IsValid)
            return Results.BadRequest(ModelState);

        try
        {
            await _accountService.CreateAccount(newAccount, cancellationToken);
            return Results.Created();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning($"Create account. {ex.Message}");
            return Results.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Create account failed.\n{ex.Message}\n{ex.StackTrace}");
            return Results.Problem(ex.Message);
        }
    }

    [Authorize(Roles = "Admin,User")]
    [HttpPut]
    [Route("/api/accounts/{id}")]
    public async Task<IResult> UpdateAccount(int id, [FromBody] UpdateAccountDto account, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update account request.");
        
        if (!ModelState.IsValid)
            return Results.BadRequest(ModelState);

        try
        {
            if (User.IsInRole("Admin")) // Admin
            {
                await _accountService.UpdateAccount(id, account, cancellationToken);
                return Results.Ok();                
            }
            else // User - check id
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                
                if (email == null) 
                    return Results.Problem("Invalid credentials");
                
                await _accountService.UpdateUsersData(email, id, account, cancellationToken);
                return Results.Ok();
            }
        }
        catch (AccessViolationException ex)
        {
            _logger.LogWarning($"Update account access violation: {ex.Message}");
            return Results.Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning($"Update account. {ex.Message}");
            return Results.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Update account failed.\n{ex.Message}\n{ex.StackTrace}");
            return Results.Problem(ex.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete]
    [Route("/api/accounts/{id}")]
    public async Task<IResult> DeleteAccount(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete account request.");
        
        try
        {
            await _accountService.DeleteAccount(id, cancellationToken);
            return Results.Ok();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning($"Delete account. {ex.Message}");
            return Results.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message + "\n" + ex.StackTrace);
            return Results.Problem(ex.Message);
        }
    }
}
