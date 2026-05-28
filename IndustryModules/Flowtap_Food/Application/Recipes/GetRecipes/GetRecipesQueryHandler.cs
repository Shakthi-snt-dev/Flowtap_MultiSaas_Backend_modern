using Flowtap_Application.Common.DTOs;
using Flowtap_Food.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Food.Application.Recipes.GetRecipes;

public class GetRecipesQueryHandler(IFoodDbContext db)
    : IRequestHandler<GetRecipesQuery, Result<List<RecipeDto>>>
{
    public async Task<Result<List<RecipeDto>>> Handle(GetRecipesQuery request, CancellationToken ct)
    {
        var recipes = await db.Recipes
            .Include(r => r.Ingredients)
            .Where(r => r.CompanyId == request.CompanyId && r.IsActive)
            .OrderBy(r => r.Name)
            .Select(r => new RecipeDto(
                r.Id,
                r.ProductId,
                r.Name,
                r.YieldQuantity,
                r.Instructions,
                r.Ingredients.Select(i => new RecipeIngredientDto(
                    i.Id,
                    i.RawMaterialProductId,
                    i.RawMaterialName,
                    i.Quantity,
                    i.Unit)).ToList()))
            .ToListAsync(ct);

        return Result<List<RecipeDto>>.Success(recipes);
    }
}
