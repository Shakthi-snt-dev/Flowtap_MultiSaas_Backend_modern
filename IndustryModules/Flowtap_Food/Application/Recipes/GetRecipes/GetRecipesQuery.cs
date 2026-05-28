using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Food.Application.Recipes.GetRecipes;

public record GetRecipesQuery(Guid CompanyId) : IRequest<Result<List<RecipeDto>>>;

public record RecipeDto(
    Guid Id,
    Guid ProductId,
    string Name,
    int YieldQuantity,
    string? Instructions,
    List<RecipeIngredientDto> Ingredients);

public record RecipeIngredientDto(
    Guid Id,
    Guid RawMaterialProductId,
    string RawMaterialName,
    decimal Quantity,
    string Unit);
