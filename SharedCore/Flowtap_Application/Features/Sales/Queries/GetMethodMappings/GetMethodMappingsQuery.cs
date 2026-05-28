using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Sales.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Sales.Queries.GetMethodMappings;

public record GetMethodMappingsQuery(Guid CompanyId, Guid LocationId)
    : IRequest<Result<List<MethodMappingDto>>>;
