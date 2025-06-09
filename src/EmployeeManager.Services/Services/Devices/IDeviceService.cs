using EmployeeManager.Services.dtos.devices;

namespace EmployeeManager.Services.Services.Devices;

public interface IDeviceService
{
    public Task<List<GetAllDeviceDto>> GetAllDevices(CancellationToken cancellationToken);
    
    public Task<GetSpecificDeviceDto?> GetDeviceById(int id, CancellationToken cancellationToken);
    
    public Task<List<GetAllDeviceTypesDto>> GetAllDeviceTypes(CancellationToken cancellationToken);

    public Task<GetSpecificDeviceDto> GetUsersDeviceById(string email, int id, CancellationToken cancellationToken);
    
    public Task<bool> CreateDevice(CreateSpecificDeviceDto createSpecificDeviceDto, CancellationToken cancellationToken);
    
    public Task<bool> UpdateDevice(int id, UpdateDeviceDto updateDeviceDto, CancellationToken cancellationToken);
    
    public Task<bool> DeleteDevice(int id, CancellationToken cancellationToken);
    
    public Task<bool> UpdateUsersDevice(string email, UpdateDeviceDto updateDeviceDto, int id, CancellationToken cancellationToken);
}