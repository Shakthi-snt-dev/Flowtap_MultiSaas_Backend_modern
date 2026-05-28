using Flowtap_Application.Features.Inventory.Commands.ApproveWriteOff;
using Flowtap_Application.Features.Inventory.Commands.CreateReorderRule;
using Flowtap_Application.Features.Inventory.Commands.UpdateReorderRule;
using Flowtap_Application.Features.Inventory.Commands.DeleteReorderRule;
using Flowtap_Application.Features.Inventory.Commands.CreateWriteOff;
using Flowtap_Application.Features.Inventory.Commands.TransferStock;
using Flowtap_Application.Features.Inventory.Commands.CreateStockBatch;
using Flowtap_Application.Features.Inventory.Commands.UpdateStockBatch;
using Flowtap_Application.Features.Inventory.Queries.GetReorderAlerts;
using Flowtap_Application.Features.Inventory.Queries.GetReorderRules;
using Flowtap_Application.Features.Inventory.Queries.GetStockLevels;
using Flowtap_Application.Features.Inventory.Queries.GetTransfers;
using Flowtap_Application.Features.Inventory.Queries.GetWriteOffs;
using Flowtap_Application.Features.Inventory.Queries.GetStockBatches;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Presentation.Controllers;

[Flowtap_Presentation.Authorization.RequirePermission("Inventory")]
[Route("api/v1/inventory")]
public class InventoryController(ISender sender) : ApiController(sender)
{
    [HttpGet("stock")]
    public async Task<IActionResult> GetStockLevels([FromQuery] GetStockLevelsQuery query, CancellationToken ct)
        => Ok(await Sender.Send(query, ct));

    // ── Transfers ──────────────────────────────────────────────────────────────
    [HttpGet("transfers")]
    public async Task<IActionResult> GetTransfers([FromQuery] GetTransfersQuery query, CancellationToken ct)
        => Ok(await Sender.Send(query with { CompanyId = CurrentTenantId }, ct));

    [HttpPost("transfers")]
    public async Task<IActionResult> Transfer([FromBody] TransferStockCommand command, CancellationToken ct)
        => Created(await Sender.Send(command, ct));

    // ── Write-offs ─────────────────────────────────────────────────────────────
    [HttpGet("write-offs")]
    public async Task<IActionResult> GetWriteOffs([FromQuery] GetWriteOffsQuery query, CancellationToken ct)
        => Ok(await Sender.Send(query with { CompanyId = CurrentTenantId }, ct));

    [HttpPost("write-offs")]
    public async Task<IActionResult> CreateWriteOff([FromBody] CreateWriteOffCommand command, CancellationToken ct)
        => Created(await Sender.Send(command, ct));

    [HttpPost("write-offs/{id:guid}/approve")]
    public async Task<IActionResult> ApproveWriteOff(Guid id, [FromBody] ApproveWriteOffCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { Id = id, CompanyId = CurrentTenantId }, ct));

    // ── Reorder Rules ──────────────────────────────────────────────────────────
    [HttpGet("reorder-rules")]
    public async Task<IActionResult> GetReorderRules(CancellationToken ct)
        => Ok(await Sender.Send(new GetReorderRulesQuery(CurrentTenantId), ct));

    [HttpPost("reorder-rules")]
    public async Task<IActionResult> CreateReorderRule([FromBody] CreateReorderRuleCommand command, CancellationToken ct)
        => Created(await Sender.Send(command, ct));

    [HttpPut("reorder-rules/{id:guid}")]
    public async Task<IActionResult> UpdateReorderRule(Guid id, [FromBody] UpdateReorderRuleCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { Id = id, CompanyId = CurrentTenantId }, ct));

    [HttpDelete("reorder-rules/{id:guid}")]
    public async Task<IActionResult> DeleteReorderRule(Guid id, CancellationToken ct)
        => FromResult(await Sender.Send(new DeleteReorderRuleCommand(id, CurrentTenantId), ct));

    [HttpGet("reorder-alerts")]
    public async Task<IActionResult> GetReorderAlerts([FromQuery] GetReorderAlertsQuery query, CancellationToken ct)
        => Ok(await Sender.Send(query, ct));

    // ── Batches ────────────────────────────────────────────────────────────────

    [HttpGet("batches")]
    public async Task<IActionResult> GetBatches([FromQuery] GetStockBatchesQuery query, CancellationToken ct)
        => Ok(await Sender.Send(query with { CompanyId = CurrentTenantId }, ct));

    [HttpPost("batches")]
    public async Task<IActionResult> CreateBatch([FromBody] CreateStockBatchCommand command, CancellationToken ct)
        => Created(await Sender.Send(command with { CompanyId = CurrentTenantId }, ct));

    [HttpPut("batches/{id:guid}")]
    public async Task<IActionResult> UpdateBatch(Guid id, [FromBody] UpdateStockBatchCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { Id = id, CompanyId = CurrentTenantId }, ct));
}
