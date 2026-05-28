using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Inventory.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Queries.GetWriteOffs;

public record GetWriteOffsQuery(
    Guid CompanyId,
    string? Status = null,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PaginatedList<WriteOffDto>>>;
