namespace EmployeeManager.Services.dtos.devices;

public class GetAllDeviceDto
{
    public int Id { get; set; }
    public string Name { get; set; }

    public GetAllDeviceDto()
    {
    }

    public GetAllDeviceDto(int id, string name)
    {
        Id = id;
        Name = name;
    }
}