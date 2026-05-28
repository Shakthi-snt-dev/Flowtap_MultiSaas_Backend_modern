using Flowtap_Repair.Domain.Entities;
using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Repair.Application.Features.Devices.Commands.RemoveProductDeviceMapping;

public record RemoveProductDeviceMappingCommand(Guid CompanyId, Guid ProductId, Guid DeviceModelId)
    : IRequest<Result<bool>>;

