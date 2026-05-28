using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Food.Application.Tables.GetTables;

public record GetTablesQuery(Guid CompanyId, Guid? LocationId) : IRequest<Result<List<FoodTableDto>>>;

public record FoodTableDto(
    Guid Id,
    Guid LocationId,
    string Name,
    int Capacity,
    string? Section,
    string Status,
    Guid? CurrentSaleId,
    bool IsActive);
