using Flowtap_Application.Common.DTOs;
using Flowtap_Hotel.Application.Reports.DTOs;
using MediatR;

namespace Flowtap_Hotel.Application.Reports;

public record GetRevParQuery(Guid CompanyId, Guid? LocationId, DateTime From, DateTime To)
    : IRequest<Result<RevParDto>>;
