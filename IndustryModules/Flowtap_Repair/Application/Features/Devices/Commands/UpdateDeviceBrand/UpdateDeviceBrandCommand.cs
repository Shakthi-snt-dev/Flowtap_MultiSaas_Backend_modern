using Flowtap_Repair.Domain.Entities;
using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Repair.Application.Features.Devices.Commands.UpdateDeviceBrand;

public record UpdateDeviceBrandCommand(
    Guid Id, Guid CompanyId,
    string? Name, string? IconUrl, string? Color, bool? IsActive) : IRequest<Result<bool>>;

