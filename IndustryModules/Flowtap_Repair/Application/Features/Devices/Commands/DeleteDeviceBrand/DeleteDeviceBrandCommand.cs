using Flowtap_Repair.Domain.Entities;
using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Repair.Application.Features.Devices.Commands.DeleteDeviceBrand;

public record DeleteDeviceBrandCommand(Guid Id, Guid CompanyId) : IRequest<Result<bool>>;

