using Flowtap_Application.Common.DTOs;
using Flowtap_Food.DbContext;
using Flowtap_Food.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Food.Application.Recipes.UpdateRecipe;

public class UpdateRecipeCommandHandler(IFoodDbContext db)
    : IRequestHandler<UpdateRecipeCommand, Result>
{
    public async Task<Result> Handle(UpdateRecipeCommand request, CancellationToken ct)
    {
        var recipe = await db.Recipes
            .Include(r => r.Ingredients)
            .FirstOrDefaultAsync(r => r.Id == request.Id && r.CompanyId == request.CompanyId, ct);

        if (recipe is null)
            return Result.Failure("Recipe not found.");

        recipe.Name          = request.Name;
        recipe.YieldQuantity = request.YieldQuantity;
        recipe.Instructions  = request.Instructions;

        // Replace ingredients
        db.RecipeIngredients.RemoveRange(recipe.Ingredients);
        recipe.Ingredients = request.Ingredients.Select(i => new RecipeIngredient
        {
            RecipeId             = recipe.Id,
            RawMaterialProductId = i.RawMaterialProductId,
            RawMaterialName      = i.RawMaterialName,
            Quantity             = i.Quantity,
            Unit                 = i.Unit
        }).ToList();

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
