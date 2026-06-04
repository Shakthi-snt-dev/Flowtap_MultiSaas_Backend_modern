using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.AuditLogs.Queries.GetAuditLogs;

public class GetAuditLogsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetAuditLogsQuery, Result<PaginatedList<AuditLogDto>>>
{
    public async Task<Result<PaginatedList<AuditLogDto>>> Handle(
        GetAuditLogsQuery request, CancellationToken ct)
    {
        var query = db.AuditLogs
            .Where(a => a.CompanyId == request.CompanyId);

        if (!string.IsNullOrWhiteSpace(request.EntityType))
            query = query.Where(a => a.EntityName == request.EntityType);

        if (!string.IsNullOrWhiteSpace(request.Action))
            query = query.Where(a => a.Action == request.Action);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(a =>
                a.EntityName.Contains(request.Search) ||
                a.EntityId.Contains(request.Search));

        var total = await query.CountAsync(ct);

        // Resolve user names in one batch query
        var page = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var userIds = page
            .Where(a => a.UserId.HasValue)
            .Select(a => a.UserId!.Value)
            .Distinct()
            .ToList();

        var userNames = userIds.Count > 0
            ? await db.UserProfiles
                .Where(p => userIds.Contains(p.UserAccountId))
                .Select(p => new { p.UserAccountId, p.Name })
                .ToDictionaryAsync(p => p.UserAccountId, p => p.Name, ct)
            : new Dictionary<Guid, string>();

        var dtos = page.Select(a => new AuditLogDto(
            a.Id,
            a.EntityName,
            a.EntityId,
            a.Action,
            a.OldValues,
            a.NewValues,
            a.ChangedColumns,
            a.UserId,
            a.UserId.HasValue && userNames.TryGetValue(a.UserId.Value, out var name) ? name : null,
            a.IpAddress,
            a.LocationId,
            a.CreatedAt)).ToList();

        return Result<PaginatedList<AuditLogDto>>.Success(
            new PaginatedList<AuditLogDto>(dtos, total, request.Page, request.PageSize));
    }
}
