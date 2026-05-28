using Flowtap_Application.Common.DTOs;
using Flowtap_Repair.Application.DTOs;
using MediatR;

namespace Flowtap_Repair.Application.Queries.GetServicesByModel;

public record GetServicesByModelQuery(Guid DeviceModelId, Guid CompanyId)
    : IRequest<Result<List<ServiceDto>>>;

