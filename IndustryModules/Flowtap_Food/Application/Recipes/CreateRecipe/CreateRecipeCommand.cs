using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Food.Application.Recipes.CreateRecipe;

public record CreateRecipeCommand(
    Guid CompanyId,
    Guid ProductId,
    string Name,
    int YieldQuantity,
    string? Instructions,
    List<CreateRecipeIngredientDto> Ingredients) : IRequest<Result<Guid>>;

public record CreateRecipeIngredientDto(
    Guid RawMaterialProductId,
    string RawMaterialName,
    decimal Quantity,
    string Unit);
