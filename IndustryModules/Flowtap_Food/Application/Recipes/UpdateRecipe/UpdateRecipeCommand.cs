using Flowtap_Application.Common.DTOs;
using Flowtap_Food.Application.Recipes.CreateRecipe;
using MediatR;

namespace Flowtap_Food.Application.Recipes.UpdateRecipe;

public record UpdateRecipeCommand(
    Guid Id,
    Guid CompanyId,
    string Name,
    int YieldQuantity,
    string? Instructions,
    List<CreateRecipeIngredientDto> Ingredients) : IRequest<Result>;
