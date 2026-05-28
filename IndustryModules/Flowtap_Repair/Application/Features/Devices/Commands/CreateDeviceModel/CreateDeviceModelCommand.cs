using Flowtap_Repair.Domain.Entities;
using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Repair.Application.Features.Devices.Commands.CreateDeviceModel;

public record DeviceModelDto(
    Guid Id,
    Guid BrandId,
    string BrandName,
    Guid? ParentModelId,
    Guid? ProductCategoryId,
    string Name,
    string? ImageUrl,
    bool IsActive);

public record CreateDeviceModelCommand(
    Guid BrandId,
    Guid? ProductCategoryId,
    Guid? ParentModelId,
    string Name,
    string? ImageUrl) : IRequest<Result<DeviceModelDto>>;

