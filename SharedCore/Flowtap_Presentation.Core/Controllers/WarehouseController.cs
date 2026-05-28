using Flowtap_Application.Features.Inventory.Commands.CreateWarehouse;
using Flowtap_Application.Features.Inventory.Commands.UpdateWarehouse;
using Flowtap_Application.Features.Inventory.Commands.DeleteWarehouse;
using Flowtap_Application.Features.Inventory.Commands.CreateWarehouseRack;
using Flowtap_Application.Features.Inventory.Commands.UpdateWarehouseRack;
using Flowtap_Application.Features.Inventory.Commands.DeleteWarehouseRack;
using Flowtap_Application.Features.Inventory.Commands.CreateWarehouseBin;
using Flowtap_Application.Features.Inventory.Commands.UpdateWarehouseBin;
using Flowtap_Application.Features.Inventory.Commands.DeleteWarehouseBin;
using Flowtap_Application.Features.Inventory.Queries.GetStockLevels;
using Flowtap_Application.Features.Inventory.Queries.GetWarehouse;
using Flowtap_Application.Features.Inventory.Queries.GetWarehouses;
using Flowtap_Application.Features.Inventory.Queries.GetWarehouseRacks;
using Flowtap_Application.Features.Inventory.Queries.GetWarehouseBins;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Presentation.Controllers;

[Flowtap_Presentation.Authorization.RequirePermission("Inventory")]
[Route("api/v1/warehouses")]
public class WarehouseController(ISender sender) : ApiController(sender)
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWarehouseCommand command, CancellationToken ct)
        => Created(await Sender.Send(command, ct));

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetWarehousesQuery query, CancellationToken ct)
        => Ok(await Sender.Send(query, ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await Sender.Send(new GetWarehouseQuery(CurrentTenantId, id), ct));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWarehouseCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { Id = id, CompanyId = CurrentTenantId }, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => Ok(await Sender.Send(new DeleteWarehouseCommand(id), ct));

    [HttpGet("{warehouseId:guid}/stock")]
    public async Task<IActionResult> GetStock(Guid warehouseId, [FromQuery] GetStockLevelsQuery query, CancellationToken ct)
    {
        query = query with { WarehouseId = warehouseId };
        return Ok(await Sender.Send(query, ct));
    }

    // ── Racks ──────────────────────────────────────────────────────────────────

    [HttpGet("{warehouseId:guid}/racks")]
    public async Task<IActionResult> GetRacks(Guid warehouseId, CancellationToken ct)
        => Ok(await Sender.Send(new GetWarehouseRacksQuery(CurrentTenantId, warehouseId), ct));

    [HttpPost("{warehouseId:guid}/racks")]
    public async Task<IActionResult> CreateRack(Guid warehouseId, [FromBody] CreateWarehouseRackCommand command, CancellationToken ct)
        => Created(await Sender.Send(command with { CompanyId = CurrentTenantId, WarehouseId = warehouseId }, ct));

    [HttpPut("{warehouseId:guid}/racks/{id:guid}")]
    public async Task<IActionResult> UpdateRack(Guid warehouseId, Guid id, [FromBody] UpdateWarehouseRackCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { Id = id, CompanyId = CurrentTenantId }, ct));

    [HttpDelete("{warehouseId:guid}/racks/{id:guid}")]
    public async Task<IActionResult> DeleteRack(Guid warehouseId, Guid id, CancellationToken ct)
        => FromResult(await Sender.Send(new DeleteWarehouseRackCommand(id, CurrentTenantId), ct));

    // ── Bins ───────────────────────────────────────────────────────────────────

    [HttpGet("{warehouseId:guid}/racks/{rackId:guid}/bins")]
    public async Task<IActionResult> GetBins(Guid warehouseId, Guid rackId, CancellationToken ct)
        => Ok(await Sender.Send(new GetWarehouseBinsQuery(CurrentTenantId, rackId), ct));

    [HttpPost("{warehouseId:guid}/racks/{rackId:guid}/bins")]
    public async Task<IActionResult> CreateBin(Guid warehouseId, Guid rackId, [FromBody] CreateWarehouseBinCommand command, CancellationToken ct)
        => Created(await Sender.Send(command with { CompanyId = CurrentTenantId, RackId = rackId }, ct));

    [HttpPut("{warehouseId:guid}/racks/{rackId:guid}/bins/{id:guid}")]
    public async Task<IActionResult> UpdateBin(Guid warehouseId, Guid rackId, Guid id, [FromBody] UpdateWarehouseBinCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { Id = id, CompanyId = CurrentTenantId }, ct));

    [HttpDelete("{warehouseId:guid}/racks/{rackId:guid}/bins/{id:guid}")]
    public async Task<IActionResult> DeleteBin(Guid warehouseId, Guid rackId, Guid id, CancellationToken ct)
        => FromResult(await Sender.Send(new DeleteWarehouseBinCommand(id, CurrentTenantId), ct));
}
