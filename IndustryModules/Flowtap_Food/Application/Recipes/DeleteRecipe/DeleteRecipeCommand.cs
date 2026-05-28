using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Food.Application.Recipes.DeleteRecipe;

public record DeleteRecipeCommand(Guid Id, Guid CompanyId) : IRequest<Result>;
