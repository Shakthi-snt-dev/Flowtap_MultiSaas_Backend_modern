using Flowtap_Application.Common.DTOs;
using Flowtap_Food.Application.Reports.DTOs;
using Flowtap_Food.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Food.Application.Reports;

public class GetRecipeProfitabilityQueryHandler(IFoodDbContext db)
    : IRequestHandler<GetRecipeProfitabilityQuery, Result<RecipeProfitabilityDto>>
{
    public async Task<Result<RecipeProfitabilityDto>> Handle(GetRecipeProfitabilityQuery request, CancellationToken ct)
    {
        var recipes = await db.Recipes
            .Where(r => r.CompanyId == request.CompanyId && r.IsActive)
            .Select(r => new
            {
                r.Id,
                r.Name,
                r.ProductId,
                Ingredients = r.Ingredients.Select(i => new
                {
                    i.Quantity,
                    i.RawMaterialProductId
                }).ToList()
            })
            .ToListAsync(ct);

        var productIds = recipes.SelectMany(r => r.Ingredients.Select(i => i.RawMaterialProductId))
            .Concat(recipes.Select(r => r.ProductId))
            .Distinct()
            .ToList();

        var products = await db.Products
            .Where(p => productIds.Contains(p.Id))
            .Select(p => new { p.Id, p.Name, p.DefaultCostPrice, p.DefaultSalePrice })
            .ToDictionaryAsync(p => p.Id, ct);

        var margins = recipes.Select(r =>
        {
            var estimatedCost = r.Ingredients.Sum(i =>
            {
                var rawMat = products.TryGetValue(i.RawMaterialProductId, out var p) ? p : null;
                return rawMat != null ? rawMat.DefaultCostPrice * i.Quantity : 0;
            });

            var finishedProduct = products.TryGetValue(r.ProductId, out var fp) ? fp : null;
            var salePrice = finishedProduct?.DefaultSalePrice ?? 0;
            var productName = finishedProduct?.Name ?? "Unknown";
            var marginPct = salePrice > 0 ? Math.Round((salePrice - estimatedCost) / salePrice * 100, 1) : 0;

            return new RecipeMarginDto(r.Id, r.Name, productName, estimatedCost, salePrice, marginPct, r.Ingredients.Count);
        })
        .OrderByDescending(r => r.MarginPercent)
        .ToList();

        return Result<RecipeProfitabilityDto>.Success(new RecipeProfitabilityDto(margins));
    }
}
