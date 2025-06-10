using EmployeeManager.Services.dtos.devices;

namespace EmployeeManager.Services.Services.Validation;

public interface IValidationService
{
    public Task<List<string>> Validate(CreateSpecificDeviceDto data);
}