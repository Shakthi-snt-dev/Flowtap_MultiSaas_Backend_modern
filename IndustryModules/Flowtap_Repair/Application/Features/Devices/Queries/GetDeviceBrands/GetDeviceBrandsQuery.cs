using Flowtap_Repair.Domain.Entities;
using Flowtap_Application.Common.DTOs;
using Flowtap_Repair.Application.Features.Devices.Commands.CreateDeviceBrand;
using MediatR;

namespace Flowtap_Repair.Application.Features.Devices.Queries.GetDeviceBrands;

public record GetDeviceBrandsQuery(
    Guid? ProductCategoryId,
    string? SearchTerm,
    bool? IsActive) : IRequest<Result<List<DeviceBrandDto>>>;

