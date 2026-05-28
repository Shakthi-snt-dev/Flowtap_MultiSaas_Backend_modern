using Flowtap_Repair.Domain.Entities;
using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Repair.Application.Features.Devices.Commands.AddProductDeviceMapping;

public record AddProductDeviceMappingCommand(Guid CompanyId, Guid ProductId, Guid DeviceModelId)
    : IRequest<Result<Guid>>;

