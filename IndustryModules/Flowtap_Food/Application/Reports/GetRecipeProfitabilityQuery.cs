using Flowtap_Application.Common.DTOs;
using MediatR;
using Flowtap_Food.Application.Reports.DTOs;

namespace Flowtap_Food.Application.Reports;

public record GetRecipeProfitabilityQuery(Guid CompanyId)
    : IRequest<Result<RecipeProfitabilityDto>>;
