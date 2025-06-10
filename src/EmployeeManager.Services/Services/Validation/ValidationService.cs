using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using EmployeeManager.Services.context;
using EmployeeManager.Services.dtos.devices;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManager.Services.Services.Validation;

public class ValidationService : IValidationService
{
    private readonly JsonElement _validations;
    private readonly EmployeeDatabaseContext _context;

    public ValidationService(EmployeeDatabaseContext context)
    {
        var jsonString = File.ReadAllText("../../validation_rules/validation_rules.json");

        using var doc = JsonDocument.Parse(jsonString);
        _validations = doc.RootElement.GetProperty("validations").Clone();
        
        _context = context;
    }

    public async Task<List<string>> Validate(CreateSpecificDeviceDto data)
    {
        try
        {
            var errors = new List<string>();
            var deviceType = await _context.DeviceTypes
                .FirstOrDefaultAsync(dt => dt.Id == data.TypeId);

            if (deviceType == null)
                throw new KeyNotFoundException($"No device type found with device type id {data.TypeId}");

            var entry = GetValidationEntry(deviceType.Name);
            
            // no rules - good!
            if (entry == null)
                return errors;

            // check properties
            var type = typeof(CreateSpecificDeviceDto);
            var property = type.GetProperty(
                    entry.Value.GetProperty("preRequestName").GetString(),
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
                );
            
            var value = property.GetValue(data);
            if (value == null)
                throw new KeyNotFoundException($"Validation failed. No field with name " +
                                               $"{entry.Value.GetProperty("preRequestName").GetString()} found.");
            
            var expectedValue = entry.Value.GetProperty("preRequestValue").GetString();
            
            // not equal values - do not apply rules
            if (!value.ToString().Equals(expectedValue, StringComparison.OrdinalIgnoreCase))
                return errors;
            
            // apply rules
            var rules = entry.Value.GetProperty("rules").EnumerateArray();
            foreach (var rule in rules)
            {
                var result = ApplyRule(data.AdditionalProperties, rule);
                if (result != null)
                    errors.Add(result);
            }

            return errors;
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Validation rules could not be validated.", ex);
        }
    }

    public JsonElement? GetValidationEntry(string deviceType)
    {
        foreach (var entry in _validations.EnumerateArray())
        {
            if (entry.GetProperty("type").GetString()!.Equals(deviceType))
                return entry;
        }
        
        return null;
    }
    
    public string? ApplyRule(object? additionalFields, JsonElement rule)
    {
        if (additionalFields == null)
            return "AdditionalProperties is null.";

        if (additionalFields is not JsonElement jsonElement)
            return "AdditionalProperties is not a valid JSON element.";

        var field = rule.GetProperty("paramName").GetString();
        
        if (!jsonElement.TryGetProperty(field!, out var fieldElement))
            return $"Missing field: {field}";

        var fieldValue = fieldElement.GetString() ?? string.Empty;
        
        var regex = rule.GetProperty("regex");

        if (regex.ValueKind == JsonValueKind.Array)
        {
            var errors = "";
            var flag = false;
            foreach (var pattern in regex.EnumerateArray())
            {
                if (!Regex.IsMatch(fieldValue, pattern.ToString()))
                    errors += $"Field '{field}' does not match regex rule.\n\t";
                else flag = true;
            }
            if (errors.Length == 0 || flag)
                return null;
            else 
                return errors;
        }
        else
        {
            var pattern = regex.GetString()?.Trim('/') ?? "";

            if (!Regex.IsMatch(fieldValue, pattern))
                return $"Field '{field}' does not match regex rule.";
        }

        return null;
    }
}