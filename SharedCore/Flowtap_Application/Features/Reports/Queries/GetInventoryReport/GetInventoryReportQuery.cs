using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Reports.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Reports.Queries.GetInventoryReport;

public record GetInventoryReportQuery(Guid CompanyId, Guid? WarehouseId = null)
    : IRequest<Result<InventoryReportDto>>;
