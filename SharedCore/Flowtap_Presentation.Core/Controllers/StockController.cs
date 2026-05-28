using Flowtap_Application.Features.Inventory.Commands.AdjustStock;
using Flowtap_Application.Features.Inventory.Queries.GetStockAdjustments;
using Flowtap_Application.Features.Inventory.Queries.GetStockLevels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Presentation.Controllers;

[Flowtap_Presentation.Authorization.RequirePermission("Inventory")]
[Route("api/v1/stock")]
public class StockController(ISender sender) : ApiController(sender)
{
    [HttpPost("adjust")]
    public async Task<IActionResult> Adjust([FromBody] AdjustStockCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command, ct));

    [HttpGet]
    public async Task<IActionResult> GetStockLevels([FromQuery] GetStockLevelsQuery query, CancellationToken ct)
        => Ok(await Sender.Send(query, ct));

    [HttpGet("adjustments")]
    public async Task<IActionResult> GetAdjustments([FromQuery] GetStockAdjustmentsQuery query, CancellationToken ct)
        => Ok(await Sender.Send(query with { CompanyId = CurrentTenantId }, ct));
}
