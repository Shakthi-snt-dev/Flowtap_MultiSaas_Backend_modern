using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using Flowtap_Food.Application.KitchenOrders.CreateKitchenOrder;
using Flowtap_Food.Application.KitchenOrders.GetKitchenOrders;
using Flowtap_Food.Application.KitchenOrders.UpdateKOTStatus;
using Flowtap_Food.Application.RawMaterials.ConsumeForProduction;
using Flowtap_Food.Application.RawMaterials.GetRawMaterials;
using Flowtap_Food.Application.Recipes.CreateRecipe;
using Flowtap_Food.Application.Recipes.DeleteRecipe;
using Flowtap_Food.Application.Recipes.GetRecipes;
using Flowtap_Food.Application.Recipes.UpdateRecipe;
using Flowtap_Food.Application.Tables.CreateTable;
using Flowtap_Food.Application.Tables.GetTables;
using Flowtap_Food.Application.Tables.UpdateTableStatus;
using Flowtap_Presentation.Authorization;
using Flowtap_Presentation.Controllers;
using Flowtap_Presentation.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Food.Controllers;

[RequiresIndustry(IndustryType.Food)]
[Route("api/v1/food")]
public class FoodController(ISender sender) : ApiController(sender)
{
    // ── Tables ────────────────────────────────────────────────────────────────

    [HttpGet("tables")]
    [RequirePermission("Food")]
    public async Task<IActionResult> GetTables([FromQuery] Guid? locationId, CancellationToken ct)
        => Ok(await Sender.Send(new GetTablesQuery(CurrentTenantId, locationId ?? CurrentLocationId), ct));

    [HttpPost("tables")]
    [RequirePermission("Food")]
    public async Task<IActionResult> CreateTable([FromBody] CreateTableCommand command, CancellationToken ct)
        => Created(await Sender.Send(command with { CompanyId = CurrentTenantId }, ct));

    [HttpPatch("tables/{id:guid}/status")]
    [RequirePermission("Food")]
    public async Task<IActionResult> UpdateTableStatus(Guid id, [FromBody] UpdateTableStatusCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { Id = id, CompanyId = CurrentTenantId }, ct));

    // ── Kitchen Orders (KOT) ──────────────────────────────────────────────────

    [HttpGet("orders")]
    [RequirePermission("Food")]
    public async Task<IActionResult> GetOrders([FromQuery] Guid? locationId, [FromQuery] string? status, CancellationToken ct)
        => Ok(await Sender.Send(new GetKitchenOrdersQuery(CurrentTenantId, locationId ?? CurrentLocationId, status), ct));

    [HttpPost("orders")]
    [RequirePermission("Food")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateKitchenOrderCommand command, CancellationToken ct)
        => Created(await Sender.Send(command with { CompanyId = CurrentTenantId }, ct));

    [HttpPatch("orders/{id:guid}/status")]
    [RequirePermission("Food")]
    public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateKOTStatusCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { Id = id, CompanyId = CurrentTenantId }, ct));

    // ── Recipes ───────────────────────────────────────────────────────────────

    [HttpGet("recipes")]
    [RequirePermission("Food")]
    public async Task<IActionResult> GetRecipes(CancellationToken ct)
        => Ok(await Sender.Send(new GetRecipesQuery(CurrentTenantId), ct));

    [HttpPost("recipes")]
    [RequirePermission("Food")]
    public async Task<IActionResult> CreateRecipe([FromBody] CreateRecipeCommand command, CancellationToken ct)
        => Created(await Sender.Send(command with { CompanyId = CurrentTenantId }, ct));

    [HttpPut("recipes/{id:guid}")]
    [RequirePermission("Food")]
    public async Task<IActionResult> UpdateRecipe(Guid id, [FromBody] UpdateRecipeCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { Id = id, CompanyId = CurrentTenantId }, ct));

    [HttpDelete("recipes/{id:guid}")]
    [RequirePermission("Food")]
    public async Task<IActionResult> DeleteRecipe(Guid id, CancellationToken ct)
        => FromResult(await Sender.Send(new DeleteRecipeCommand(id, CurrentTenantId), ct));

    // ── Raw Materials ─────────────────────────────────────────────────────────

    [HttpGet("raw-materials")]
    [RequirePermission("Food")]
    public async Task<IActionResult> GetRawMaterials([FromQuery] Guid? warehouseId, CancellationToken ct)
        => Ok(await Sender.Send(new GetRawMaterialsQuery(CurrentTenantId, warehouseId), ct));

    [HttpPost("raw-materials/consume")]
    [RequirePermission("Food")]
    public async Task<IActionResult> ConsumeForProduction([FromBody] ConsumeForProductionCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { CompanyId = CurrentTenantId }, ct));
}
