namespace EmployeeManager.Services.dtos.devices;

public class GetSpecificDeviceDto
{
    public string Name { get; set; }
    
    public bool IsEnabled { get; set; }
    public object? AdditionalProperties { get; set; }
    
    public string Type { get; set; }
}