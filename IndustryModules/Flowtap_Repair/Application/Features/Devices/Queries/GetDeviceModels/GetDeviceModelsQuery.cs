using Flowtap_Repair.Domain.Entities;
using Flowtap_Application.Common.DTOs;
using Flowtap_Repair.Application.Features.Devices.Commands.CreateDeviceModel;
using MediatR;

namespace Flowtap_Repair.Application.Features.Devices.Queries.GetDeviceModels;

public record GetDeviceModelsQuery(
    Guid? BrandId,
    Guid? ProductCategoryId,
    Guid? ParentModelId,
    string? SearchTerm,
    bool? IsActive) : IRequest<Result<List<DeviceModelDto>>>;

