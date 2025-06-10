﻿using System.ComponentModel.DataAnnotations;

namespace EmployeeManager.Services.dtos.devices;

public class UpdateDeviceDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    public bool IsEnabled { get; set; }

    public object? AdditionalProperties { get; set; }

    [Required]
    public int TypeId { get; set; }
}