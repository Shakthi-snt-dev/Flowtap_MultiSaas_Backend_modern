using Flowtap_Repair.Domain.Entities;
using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Repair.Application.Features.Devices.Commands.DeleteDeviceModel;

public record DeleteDeviceModelCommand(Guid Id, Guid CompanyId) : IRequest<Result<bool>>;

