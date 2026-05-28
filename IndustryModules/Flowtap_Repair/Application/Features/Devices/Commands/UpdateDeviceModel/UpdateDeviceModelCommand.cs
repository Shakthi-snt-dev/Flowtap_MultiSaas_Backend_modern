using Flowtap_Repair.Domain.Entities;
using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Repair.Application.Features.Devices.Commands.UpdateDeviceModel;

public record UpdateDeviceModelCommand(
    Guid Id, Guid CompanyId,
    string? Name, string? ImageUrl, bool? IsActive) : IRequest<Result<bool>>;

