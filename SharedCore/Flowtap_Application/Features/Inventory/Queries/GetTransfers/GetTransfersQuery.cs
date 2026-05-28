using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Inventory.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Queries.GetTransfers;

public record GetTransfersQuery(
    Guid CompanyId,
    string? Status = null,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PaginatedList<TransferDto>>>;
