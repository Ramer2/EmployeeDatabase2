using System.Text;
using System.Text.Json;
using EmployeeManager.Services.dtos.devices;
using EmployeeManager.Services.Services.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EmployeeManager.Services.Helpers.Middleware;

public class ValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ValidationMiddleware> _logger;

    public ValidationMiddleware(RequestDelegate next, ILogger<ValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // get the service
            var validationService = context.RequestServices.GetRequiredService<IValidationService>();
            
            if (!((context.Request.Method == "POST" || context.Request.Method == "PUT") 
                  && context.Request.Path.StartsWithSegments("/api/devices")))
            {
                await _next(context);
                return;
            }

            if (context.Request.Method == "POST")
            {
                _logger.LogInformation("The given request is a POST /api/devices. Deserializing...");                
            } else if (context.Request.Method == "PUT")
            {
                _logger.LogInformation("The given request is a PUT /api/devices. Deserializing...");   
            }
            
            context.Request.EnableBuffering();

            context.Request.Body.Position = 0;

            using var reader = new StreamReader(
                context.Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 1024,
                leaveOpen: true);

            var bodyAsText = await reader.ReadToEndAsync();

            context.Request.Body.Position = 0;

            CreateSpecificDeviceDto jsonObject;

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (!string.IsNullOrWhiteSpace(bodyAsText) &&
                context.Request.ContentType?.Contains("application/json") == true)
            {
                jsonObject = JsonSerializer.Deserialize<CreateSpecificDeviceDto>(bodyAsText, options);
                _logger.LogInformation($"Deserialization was succesful, body:\n\t{bodyAsText}");

                _logger.LogInformation("Validating the request...");
                var result = await validationService.Validate(jsonObject);

                if (result.Count > 0)
                {
                    var errors = "";
                    foreach (var error in result)
                    {
                        errors += $"\t{error}\n";
                    }

                    throw new ApplicationException("Validation failed: \n" + errors);
                }
            }
            else
            {
                throw new JsonException("Invalid request body.");
            }

            await _next(context);
        }
        catch (JsonException ex)
        {
            context.Response.StatusCode = 415;
            await context.Response.WriteAsync(ex.Message);
            _logger.LogInformation("The request could not be deserialized.");
            _logger.LogError(ex, ex.Message);
        }
        catch (ApplicationException ex)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync(ex.Message);
            _logger.LogError($"Validation failed: {ex.Message}");
        }
        catch (KeyNotFoundException ex)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync(ex.Message);
            _logger.LogError($"Validation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync(ex.Message);
            _logger.LogError($"Unforseen error: {ex.Message}");
        }
    }
}