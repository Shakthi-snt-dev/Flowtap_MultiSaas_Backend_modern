using Flowtap_Application.Features.Purchase.Commands.CreatePurchaseOrder;
using Flowtap_Application.Features.Purchase.Commands.CreateSupplier;
using Flowtap_Application.Features.Purchase.Commands.UpdateSupplier;
using Flowtap_Application.Features.Purchase.Commands.UpdatePurchaseOrderStatus;
using Flowtap_Application.Features.Purchase.Queries.GetPurchaseOrder;
using Flowtap_Application.Features.Purchase.Queries.GetPurchaseOrders;
using Flowtap_Application.Features.Purchase.Queries.GetSuppliers;
using Flowtap_Application.Features.Purchase.Queries.GetSupplier;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Presentation.Controllers;

[Flowtap_Presentation.Authorization.RequirePermission("Purchasing")]
[Route("api/v1/purchases")]
public class PurchaseController(ISender sender) : ApiController(sender)
{
    // ── Orders ──────────────────────────────────────────────────────────────

    [HttpPost("orders")]
    public async Task<IActionResult> CreateOrder([FromBody] CreatePurchaseOrderCommand command, CancellationToken ct)
        => Created(await Sender.Send(command, ct));

    [HttpGet("orders")]
    public async Task<IActionResult> GetOrders([FromQuery] GetPurchaseOrdersQuery query, CancellationToken ct)
        => Ok(await Sender.Send(query, ct));

    [HttpGet("orders/{id:guid}")]
    public async Task<IActionResult> GetOrder(Guid id, CancellationToken ct)
        => Ok(await Sender.Send(new GetPurchaseOrderQuery(CurrentTenantId, id), ct));

    [HttpPatch("orders/{id:guid}/status")]
    public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusRequest body, CancellationToken ct)
        => Ok(await Sender.Send(new UpdatePurchaseOrderStatusCommand(id, CurrentTenantId, body.Status), ct));

    // ── Suppliers ────────────────────────────────────────────────────────────

    [HttpPost("suppliers")]
    public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierCommand command, CancellationToken ct)
        => Created(await Sender.Send(command, ct));

    [HttpGet("suppliers")]
    public async Task<IActionResult> GetSuppliers([FromQuery] GetSuppliersQuery query, CancellationToken ct)
        => Ok(await Sender.Send(query, ct));

    [HttpGet("suppliers/{id:guid}")]
    public async Task<IActionResult> GetSupplier(Guid id, CancellationToken ct)
        => Ok(await Sender.Send(new GetSupplierQuery(CurrentTenantId, id), ct));

    [HttpPut("suppliers/{id:guid}")]
    public async Task<IActionResult> UpdateSupplier(Guid id, [FromBody] UpdateSupplierCommand command, CancellationToken ct)
    {
        command = command with { SupplierId = id, CompanyId = CurrentTenantId };
        return Ok(await Sender.Send(command, ct));
    }
}

public record UpdateOrderStatusRequest(string Status);
