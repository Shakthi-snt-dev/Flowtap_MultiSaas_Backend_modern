using Flowtap_Application.Features.Sales.Commands.CreateMethodMapping;
using Flowtap_Application.Features.Sales.Commands.CreatePaymentAccount;
using Flowtap_Application.Features.Sales.Commands.DeleteMethodMapping;
using Flowtap_Application.Features.Sales.Commands.DeletePaymentAccount;
using Flowtap_Application.Features.Sales.Commands.UpdatePaymentAccount;
using Flowtap_Application.Features.Sales.Queries.GetMethodMappings;
using Flowtap_Application.Features.Sales.Queries.GetPaymentAccounts;
using Flowtap_Application.Features.Sales.Queries.GetPayments;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Presentation.Controllers;

[Route("api/v1/payments")]
public class PaymentController(ISender sender) : ApiController(sender)
{
    // ── Payment History ───────────────────────────────────────────────────────

    // GET /api/v1/payments?page=1&pageSize=30&method=Cash&purpose=Advance
    [HttpGet]
    public async Task<IActionResult> GetPayments(
        [FromQuery] Guid? locationId,
        [FromQuery] Guid? ticketId,
        [FromQuery] Guid? saleId,
        [FromQuery] string? method,
        [FromQuery] string? purpose,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 30,
        CancellationToken ct = default)
        => Ok(await Sender.Send(
            new GetPaymentsQuery(CurrentTenantId, locationId, ticketId, saleId, method, purpose, dateFrom, dateTo, page, pageSize), ct));

    // ── Payment Accounts ──────────────────────────────────────────────────────

    // GET /api/v1/payments/accounts
    [HttpGet("accounts")]
    public async Task<IActionResult> GetAccounts([FromQuery] GetPaymentAccountsQuery query, CancellationToken ct)
        => Ok(await Sender.Send(query, ct));

    // POST /api/v1/payments/accounts
    [HttpPost("accounts")]
    public async Task<IActionResult> CreateAccount([FromBody] CreatePaymentAccountCommand command, CancellationToken ct)
        => Created(await Sender.Send(command, ct));

    // PUT /api/v1/payments/accounts/{id}
    [HttpPut("accounts/{id:guid}")]
    public async Task<IActionResult> UpdateAccount(Guid id, [FromBody] UpdatePaymentAccountCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { Id = id, CompanyId = CurrentTenantId }, ct));

    // DELETE /api/v1/payments/accounts/{id}
    [HttpDelete("accounts/{id:guid}")]
    public async Task<IActionResult> DeleteAccount(Guid id, CancellationToken ct)
        => FromResult(await Sender.Send(new DeletePaymentAccountCommand(id, CurrentTenantId), ct));

    // ── Method Mappings ───────────────────────────────────────────────────────

    // GET /api/v1/payments/method-mappings?locationId=xxx
    [HttpGet("method-mappings")]
    public async Task<IActionResult> GetMappings([FromQuery] Guid locationId, CancellationToken ct)
        => Ok(await Sender.Send(new GetMethodMappingsQuery(CurrentTenantId, locationId), ct));

    // POST /api/v1/payments/method-mappings
    [HttpPost("method-mappings")]
    public async Task<IActionResult> CreateMapping([FromBody] CreateMethodMappingCommand command, CancellationToken ct)
        => Created(await Sender.Send(command with { CompanyId = CurrentTenantId }, ct));

    // DELETE /api/v1/payments/method-mappings/{id}
    [HttpDelete("method-mappings/{id:guid}")]
    public async Task<IActionResult> DeleteMapping(Guid id, CancellationToken ct)
        => FromResult(await Sender.Send(new DeleteMethodMappingCommand(id, CurrentTenantId), ct));
}
