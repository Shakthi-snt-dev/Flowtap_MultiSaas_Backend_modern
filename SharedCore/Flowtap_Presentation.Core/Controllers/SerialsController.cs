using Flowtap_Application.Features.Inventory.Commands.CreateInventorySerial;
using Flowtap_Application.Features.Inventory.Commands.UpdateInventorySerial;
using Flowtap_Application.Features.Inventory.Queries.GetInventorySerials;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Presentation.Controllers;

[Flowtap_Presentation.Authorization.RequirePermission("Inventory")]
[Route("api/v1/serials")]
public class SerialsController(ISender sender) : ApiController(sender)
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetInventorySerialsQuery query, CancellationToken ct)
        => Ok(await Sender.Send(query with { CompanyId = CurrentTenantId }, ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInventorySerialCommand command, CancellationToken ct)
        => Created(await Sender.Send(command with { CompanyId = CurrentTenantId }, ct));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateInventorySerialCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { Id = id, CompanyId = CurrentTenantId }, ct));
}
