using Flowtap_Application.Features.Inventory.Commands.CreateBarcodeTemplate;
using Flowtap_Application.Features.Inventory.Commands.UpdateBarcodeTemplate;
using Flowtap_Application.Features.Inventory.Commands.DeleteBarcodeTemplate;
using Flowtap_Application.Features.Inventory.Commands.UpdateInventorySettings;
using Flowtap_Application.Features.Inventory.Commands.UpsertLocationInventorySettings;
using Flowtap_Application.Features.Inventory.Queries.GetBarcodeTemplates;
using Flowtap_Application.Features.Inventory.Queries.GetInventorySettings;
using Flowtap_Application.Features.Inventory.Queries.GetLocationInventorySettings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Presentation.Controllers;

[Route("api/v1/inventory/settings")]
public class InventorySettingsController(ISender sender) : ApiController(sender)
{
    // ── General Settings ───────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
        => Ok(await Sender.Send(new GetInventorySettingsQuery(CurrentTenantId), ct));

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateInventorySettingsCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { CompanyId = CurrentTenantId }, ct));

    // ── Location Settings ──────────────────────────────────────────────────────

    [HttpGet("locations/{locationId:guid}")]
    public async Task<IActionResult> GetLocationSettings(Guid locationId, CancellationToken ct)
        => Ok(await Sender.Send(new GetLocationInventorySettingsQuery(CurrentTenantId, locationId), ct));

    [HttpPut("locations/{locationId:guid}")]
    public async Task<IActionResult> UpsertLocationSettings(Guid locationId, [FromBody] UpsertLocationInventorySettingsCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { CompanyId = CurrentTenantId, LocationId = locationId }, ct));

    // ── Barcode Templates ──────────────────────────────────────────────────────

    [HttpGet("barcode-templates")]
    public async Task<IActionResult> GetBarcodeTemplates(CancellationToken ct)
        => Ok(await Sender.Send(new GetBarcodeTemplatesQuery(CurrentTenantId), ct));

    [HttpPost("barcode-templates")]
    public async Task<IActionResult> CreateBarcodeTemplate([FromBody] CreateBarcodeTemplateCommand command, CancellationToken ct)
        => Created(await Sender.Send(command with { CompanyId = CurrentTenantId }, ct));

    [HttpPut("barcode-templates/{id:guid}")]
    public async Task<IActionResult> UpdateBarcodeTemplate(Guid id, [FromBody] UpdateBarcodeTemplateCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { Id = id, CompanyId = CurrentTenantId }, ct));

    [HttpDelete("barcode-templates/{id:guid}")]
    public async Task<IActionResult> DeleteBarcodeTemplate(Guid id, CancellationToken ct)
        => FromResult(await Sender.Send(new DeleteBarcodeTemplateCommand(id, CurrentTenantId), ct));
}
