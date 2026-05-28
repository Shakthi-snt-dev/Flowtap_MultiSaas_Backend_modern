using Flowtap_Repair.Domain.Entities;
using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Repair.Application.Features.Devices.Commands.CreateDeviceBrand;

public record DeviceBrandDto(
    Guid Id,
    Guid? ProductCategoryId,
    string Name,
    string? IconUrl,
    string? Color,
    bool IsActive);

public record CreateDeviceBrandCommand(
    Guid? ProductCategoryId,
    string Name,
    string? IconUrl,
    string? Color) : IRequest<Result<DeviceBrandDto>>;

