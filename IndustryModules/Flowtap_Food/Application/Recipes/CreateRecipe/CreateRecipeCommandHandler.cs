using Flowtap_Application.Common.DTOs;
using Flowtap_Food.DbContext;
using Flowtap_Food.Domain.Entities;
using MediatR;

namespace Flowtap_Food.Application.Recipes.CreateRecipe;

public class CreateRecipeCommandHandler(IFoodDbContext db)
    : IRequestHandler<CreateRecipeCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateRecipeCommand request, CancellationToken ct)
    {
        var recipe = new Recipe
        {
            CompanyId    = request.CompanyId,
            ProductId    = request.ProductId,
            Name         = request.Name,
            YieldQuantity = request.YieldQuantity,
            Instructions = request.Instructions,
            Ingredients  = request.Ingredients.Select(i => new RecipeIngredient
            {
                RawMaterialProductId = i.RawMaterialProductId,
                RawMaterialName      = i.RawMaterialName,
                Quantity             = i.Quantity,
                Unit                 = i.Unit
            }).ToList()
        };

        db.Recipes.Add(recipe);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(recipe.Id);
    }
}
