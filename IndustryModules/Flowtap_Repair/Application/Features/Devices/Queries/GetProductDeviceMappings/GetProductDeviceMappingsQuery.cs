using Flowtap_Repair.Domain.Entities;
using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Repair.Application.Features.Devices.Queries.GetProductDeviceMappings;

public record ProductDeviceMappingDto(Guid DeviceModelId, string ModelName, string? BrandName);

public record GetProductDeviceMappingsQuery(Guid CompanyId, Guid ProductId)
    : IRequest<Result<List<ProductDeviceMappingDto>>>;

