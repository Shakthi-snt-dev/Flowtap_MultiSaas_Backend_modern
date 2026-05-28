using Flowtap_Application.Common.DTOs;
using Flowtap_Repair.Application.DTOs;
using MediatR;

namespace Flowtap_Repair.Application.Queries.GetService;

public record GetServiceQuery(Guid CompanyId, Guid ServiceId) : IRequest<Result<ServiceDto>>;

