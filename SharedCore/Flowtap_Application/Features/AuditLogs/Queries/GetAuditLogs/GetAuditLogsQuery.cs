using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.AuditLogs.Queries.GetAuditLogs;

public record AuditLogDto(
    Guid   Id,
    string EntityName,
    string EntityId,
    string Action,           // Create | Update | Delete
    string? OldValues,       // JSON string
    string? NewValues,       // JSON string
    string? ChangedColumns,
    Guid?  UserId,
    string? PerformedByName, // resolved from UserAccount
    string? IpAddress,
    Guid?  LocationId,
    DateTime CreatedAt);

public record GetAuditLogsQuery(
    Guid    CompanyId,
    string? EntityType  = null,   // filter by EntityName (e.g. "ServiceTicket")
    string? Action      = null,   // filter by Action (e.g. "Create")
    string? Search      = null,   // search in EntityName / EntityId
    int     Page        = 1,
    int     PageSize    = 30)
    : IRequest<Result<PaginatedList<AuditLogDto>>>;
