using Flowtap_Application.Common.DTOs;
using Flowtap_Food.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Food.Application.Recipes.DeleteRecipe;

public class DeleteRecipeCommandHandler(IFoodDbContext db)
    : IRequestHandler<DeleteRecipeCommand, Result>
{
    public async Task<Result> Handle(DeleteRecipeCommand request, CancellationToken ct)
    {
        var recipe = await db.Recipes
            .FirstOrDefaultAsync(r => r.Id == request.Id && r.CompanyId == request.CompanyId, ct);

        if (recipe is null)
            return Result.Failure("Recipe not found.");

        recipe.IsActive = false;
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
