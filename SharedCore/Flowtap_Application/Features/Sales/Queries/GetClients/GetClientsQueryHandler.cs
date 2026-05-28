using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Sales.DTOs;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Sales.Queries.GetClients;

public class GetClientsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetClientsQuery, Result<PaginatedList<ClientListItemDto>>>
{
    public async Task<Result<PaginatedList<ClientListItemDto>>> Handle(GetClientsQuery request, CancellationToken ct)
    {
        var query = db.Clients.Where(c => c.CompanyId == request.CompanyId && c.IsActive);

        if (request.LocationId.HasValue)
            query = query.Where(c => c.LocationId == request.LocationId.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(c => c.Name.Contains(request.Search)
                || (c.Phone != null && c.Phone.Contains(request.Search))
                || (c.Email != null && c.Email.Contains(request.Search)));

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(c => c.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        if (items.Count == 0)
            return Result<PaginatedList<ClientListItemDto>>.Success(
                new PaginatedList<ClientListItemDto>([], total, request.Page, request.PageSize));

        var clientIds = items.Select(c => c.Id).ToList();

        // Batch load sales stats for all clients on this page
        var salesStats = await db.Sales
            .Where(s => s.CompanyId == request.CompanyId
                     && s.ClientId != null && clientIds.Contains(s.ClientId.Value)
                     && s.Status == SaleStatus.Completed)
            .GroupBy(s => s.ClientId)
            .Select(g => new
            {
                ClientId    = g.Key,
                TotalSpent  = g.Sum(s => s.TotalAmount),
                VisitCount  = g.Count(),
                LastVisitAt = g.Max(s => (DateTime?)s.CreatedAt),
            })
            .ToListAsync(ct);

        // Batch load total payments for all clients
        var paymentStats = await db.Payments
            .Where(p => p.Sale != null
                     && p.Sale.CompanyId == request.CompanyId
                     && p.Sale.ClientId != null && clientIds.Contains(p.Sale.ClientId.Value))
            .GroupBy(p => p.Sale!.ClientId)
            .Select(g => new { ClientId = g.Key, TotalPaid = g.Sum(p => p.Amount) })
            .ToListAsync(ct);

        var statsMap   = salesStats.ToDictionary(s => s.ClientId!.Value);
        var paymentMap = paymentStats.ToDictionary(p => p.ClientId!.Value);

        var dtos = items.Select(c =>
        {
            statsMap.TryGetValue(c.Id, out var stats);
            paymentMap.TryGetValue(c.Id, out var pmt);
            return new ClientListItemDto(
                c.Id, c.Name, c.Phone, c.Email, c.Type.ToString(), c.IsActive,
                LocationId  : c.LocationId,
                TotalSpent  : stats?.TotalSpent  ?? 0,
                TotalPaid   : pmt?.TotalPaid     ?? 0,
                VisitCount  : stats?.VisitCount  ?? 0,
                LastVisitAt : stats?.LastVisitAt,
                WhatsApp    : c.WhatsApp,
                CompanyName : c.CompanyName,
                GSTIN       : c.GSTIN,
                Address     : c.AddressLine1,
                City        : c.City,
                State       : c.State,
                PostalCode  : c.PostalCode,
                DiscountPercent : c.DiscountPercent,
                DateOfBirth : c.DateOfBirth,
                ReferralSource : c.ReferralSource,
                Notes       : c.Notes);
        }).ToList();

        return Result<PaginatedList<ClientListItemDto>>.Success(
            new PaginatedList<ClientListItemDto>(dtos, total, request.Page, request.PageSize));
    }
}
