using System.Text.Json;
using EmployeeManager.Models.models;
using EmployeeManager.Services.context;
using EmployeeManager.Services.dtos.devices;
using EmployeeManager.Services.dtos.employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManager.Services.Services.Devices;

public class DeviceService : IDeviceService
{
    private EmployeeDatabaseContext _context;

    public DeviceService(EmployeeDatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<GetAllDeviceDto>> GetAllDevices(CancellationToken cancellationToken)
    {
        try
        {
            var deviceDtos = new List<GetAllDeviceDto>();
            var devices = await _context.Devices.ToListAsync(cancellationToken);
            // mapping to dtos
            foreach (var device in devices)
            {
                deviceDtos.Add(new GetAllDeviceDto
                {
                    Id = device.Id,
                    Name = device.Name
                });
            }

            return deviceDtos;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error retrieving devices", ex);
        }
    }

    public async Task<GetSpecificDeviceDto?> GetDeviceById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var device = await _context.Devices
                .Include(d => d.DeviceType)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            
            if (device == null) return null;
        
            var deviceDto = new GetSpecificDeviceDto
            {
                Name = device.Name,
                AdditionalProperties = device.AdditionalProperties.IsNullOrEmpty() ? null : JsonDocument.Parse(device.AdditionalProperties).RootElement,
                Type = device.DeviceType.Name,
            };
            
            return deviceDto;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error retrieving devices", ex);
        }
    }
    
    public async Task<List<GetAllDeviceTypesDto>> GetAllDeviceTypes(CancellationToken cancellationToken)
    {
        try
        {
            var deviceTypes = await _context.DeviceTypes.ToListAsync(cancellationToken);
            var deviceTypesDtos = new List<GetAllDeviceTypesDto>();

            foreach (var deviceType in deviceTypes)
            {
                deviceTypesDtos.Add(new GetAllDeviceTypesDto
                {
                    Id = deviceType.Id,
                    Name = deviceType.Name
                });
            }
            
            return deviceTypesDtos;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Problem retrieving device types");
        }
    }

    public async Task<GetSpecificDeviceDto> GetUsersDeviceById(string email, int id, CancellationToken cancellationToken)
    {
        try
        {
            var employee = await _context.Employees
                .Include(e => e.Person)
                .Where(emp => emp.Person.Email.Equals(email))
                .FirstOrDefaultAsync(cancellationToken);

            if (employee == null)
                throw new KeyNotFoundException($"No employee found with email {email}");

            var device = await GetDeviceById(id, cancellationToken);

            var deCheck = await _context.DeviceEmployees
                .Where(de => de.Device.Id == id && de.Employee.Id == employee.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (deCheck == null)
                throw new AccessViolationException($"User cannot access devices, which are not theirs");

            return device;
        }
        catch (AccessViolationException)
        {
            throw;
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error retrieving devices", ex);
        }
    }

    public async Task<bool> CreateDevice(CreateSpecificDeviceDto createSpecificDeviceDto, CancellationToken cancellationToken)
    {
        if (createSpecificDeviceDto.Name == null)
        {
            throw new ArgumentException("Invalid device name");
        }

        try
        {
            var deviceType = await _context.DeviceTypes
                .FirstOrDefaultAsync(x => x.Id == createSpecificDeviceDto.TypeId, cancellationToken);

            if (deviceType == null)
            {
                throw new ArgumentException("Invalid device type");
            }

            var device = new Device
            {
                Name = createSpecificDeviceDto.Name,
                DeviceType = deviceType,
                IsEnabled = createSpecificDeviceDto.IsEnabled,
                AdditionalProperties =
                    (createSpecificDeviceDto.AdditionalProperties == null ? "" : createSpecificDeviceDto.AdditionalProperties)
                    .ToString()
            };

            await _context.Devices.AddAsync(device, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error creating a device.", ex);
        }
    }
    
    public async Task<bool> UpdateDevice(int id, UpdateDeviceDto updateDeviceDto, CancellationToken cancellationToken)
    {
        if (updateDeviceDto.Name == null)
        {
            throw new ArgumentException("Invalid device name");
        }

        if (updateDeviceDto.DeviceType == null)
        {
            throw new ArgumentException("Invalid device type");
        }

        try
        {
            var device = await _context.Devices
                .Include(d => d.DeviceType)
                .Include(de => de.DeviceEmployees)
                .ThenInclude(emp => emp.Employee)
                .ThenInclude(p => p.Person)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            
            if (device == null)
                throw new KeyNotFoundException("Device not found");

            var deviceType = await _context.DeviceTypes
                .FirstOrDefaultAsync(x => x.Name == updateDeviceDto.DeviceType, cancellationToken);
            
            if (deviceType == null)
            {
                throw new ArgumentException("Invalid device type");
            }
            
            var updateDevice = new Device
            {
                Name = updateDeviceDto.Name,
                IsEnabled = updateDeviceDto.IsEnabled,
                DeviceType = deviceType,
                AdditionalProperties = (updateDeviceDto.AdditionalProperties == null ? "" : updateDeviceDto.AdditionalProperties).ToString()
            };

            device.Name = updateDevice.Name;
            device.IsEnabled = updateDevice.IsEnabled;
            device.DeviceType = updateDevice.DeviceType;
            device.AdditionalProperties = updateDevice.AdditionalProperties;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error updating device with id {id}", ex);
        }
    }

    public async Task<bool> DeleteDevice(int id, CancellationToken cancellationToken)
    {
        var deviceCheck = await GetDeviceById(id, cancellationToken);
        if (deviceCheck == null) 
            throw new KeyNotFoundException("Device not found");
        
        try
        {
            _context.Devices.Remove(_context.Devices.FirstOrDefault(x => x.Id == id));
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error deleting device with id {id}", ex);
        }
    }

    public async Task<bool> UpdateUsersDevice(string email, UpdateDeviceDto updateDeviceDto, int id, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _context.Employees
                .Include(emp => emp.Person)
                .Include(emp => emp.DeviceEmployees)
                .ThenInclude(de => de.Device)
                .Where(emp => emp.Person.Email == email)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
                throw new KeyNotFoundException($"No user found with email {email}");

            var device = user.DeviceEmployees.FirstOrDefault(de => de.Device.Id == id);

            if (device == null)
                throw new KeyNotFoundException($"Device with id {id} not found");

            var deviceType = await _context.DeviceTypes
                .FirstOrDefaultAsync(x => x.Name == updateDeviceDto.DeviceType, cancellationToken);

            if (deviceType == null)
                throw new AccessViolationException($"User cannot update devices which do not belong to them.");

            device.Device.Name = updateDeviceDto.Name;
            device.Device.DeviceType = deviceType;
            device.Device.IsEnabled = updateDeviceDto.IsEnabled;
            device.Device.AdditionalProperties =
                (updateDeviceDto.AdditionalProperties == null ? "" : updateDeviceDto.AdditionalProperties).ToString();

            _context.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (AccessViolationException)
        {
            throw;
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error updating device for email {email}", ex);
        }
    }
}