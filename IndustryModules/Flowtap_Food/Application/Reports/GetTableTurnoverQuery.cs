using Flowtap_Application.Common.DTOs;
using MediatR;
using Flowtap_Food.Application.Reports.DTOs;

namespace Flowtap_Food.Application.Reports;

public record GetTableTurnoverQuery(Guid CompanyId, Guid? LocationId, DateTime From, DateTime To)
    : IRequest<Result<TableTurnoverDto>>;
