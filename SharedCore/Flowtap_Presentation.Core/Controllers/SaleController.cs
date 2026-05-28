using Flowtap_Application.Features.Sales.Commands.CreateSale;
using Flowtap_Application.Features.Sales.Commands.AddPayment;
using Flowtap_Application.Features.Sales.Commands.VoidSale;
using Flowtap_Application.Features.Sales.Queries.GetSales;
using Flowtap_Application.Features.Sales.Queries.GetSale;
using Flowtap_Presentation.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Presentation.Controllers;

[RequirePermission("POS")]
[Route("api/v1/sales")]
public class SaleController(ISender sender) : ApiController(sender)
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSaleCommand command, CancellationToken ct)
        => Created(await Sender.Send(command, ct));

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetSalesQuery query, CancellationToken ct)
        => Ok(await Sender.Send(query, ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOne(Guid id, CancellationToken ct)
        => Ok(await Sender.Send(new GetSaleQuery(CurrentTenantId, id), ct));

    [HttpPost("{id:guid}/void")]
    public async Task<IActionResult> Void(Guid id, [FromBody] VoidSaleRequest body, CancellationToken ct)
        => Ok(await Sender.Send(new VoidSaleCommand(id, CurrentTenantId, body.Reason), ct));

    [HttpPost("{saleId:guid}/payments")]
    public async Task<IActionResult> AddPayment(Guid saleId, [FromBody] AddPaymentCommand command, CancellationToken ct)
    {
        command = command with { SaleId = saleId };
        return Created(await Sender.Send(command, ct));
    }
}

public record VoidSaleRequest(string? Reason);
