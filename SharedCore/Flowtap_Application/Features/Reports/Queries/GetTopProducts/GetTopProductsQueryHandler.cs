using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Reports.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Reports.Queries.GetTopProducts;

public class GetTopProductsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetTopProductsQuery, Result<TopProductsReportDto>>
{
    public async Task<Result<TopProductsReportDto>> Handle(GetTopProductsQuery request, CancellationToken ct)
    {
        var saleItemsQuery = db.SaleItems
            .Include(si => si.Sale)
            .Where(si => si.Sale.CompanyId == request.CompanyId
                && si.Sale.CreatedAt >= request.From
                && si.Sale.CreatedAt <= request.To);

        if (request.LocationId.HasValue)
            saleItemsQuery = saleItemsQuery.Where(si => si.Sale.LocationId == request.LocationId.Value);

        var grouped = await saleItemsQuery
            .GroupBy(si => new { si.ProductId, si.ProductName })
            .Select(g => new
            {
                g.Key.ProductId,
                g.Key.ProductName,
                QuantitySold = (int)g.Sum(si => si.Quantity),
                Revenue = g.Sum(si => si.Total)
            })
            .OrderByDescending(x => x.Revenue)
            .Take(request.Top)
            .ToListAsync(ct);

        // Enrich with SKU from Products table
        var productIds = grouped.Select(x => x.ProductId).ToList();
        var skuMap = await db.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p.SKU, ct);

        var products = grouped
            .Select(x => new TopProductDto(x.ProductId, x.ProductName, skuMap.GetValueOrDefault(x.ProductId, string.Empty), x.QuantitySold, x.Revenue))
            .ToList();

        return Result<TopProductsReportDto>.Success(new TopProductsReportDto(request.From, request.To, products));
    }
}
