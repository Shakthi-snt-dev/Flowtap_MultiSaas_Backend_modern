using Flowtap_Application.Features.Sales.Commands.CreateClient;
using Flowtap_Application.Features.Sales.Commands.UpdateClient;
using Flowtap_Application.Features.Sales.Queries.GetClient;
using Flowtap_Application.Features.Sales.Queries.GetClientPurchases;
using Flowtap_Application.Features.Sales.Queries.GetClients;
using Flowtap_Presentation.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Presentation.Controllers;

[RequirePermission("Clients")]
[Route("api/v1/clients")]
public class ClientController(ISender sender) : ApiController(sender)
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClientCommand command, CancellationToken ct)
    {
        var locationId = command.LocationId != Guid.Empty
            ? command.LocationId
            : CurrentStoreId ?? CurrentLocationId ?? Guid.Empty;
        return Created(await Sender.Send(command with { CompanyId = CurrentTenantId, LocationId = locationId }, ct));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetClientsQuery query, CancellationToken ct)
    {
        var locationId = query.LocationId ?? CurrentStoreId ?? CurrentLocationId;
        return Ok(await Sender.Send(query with { CompanyId = CurrentTenantId, LocationId = locationId }, ct));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await Sender.Send(new GetClientQuery(CurrentTenantId, id), ct));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClientCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { Id = id, CompanyId = CurrentTenantId }, ct));

    [HttpGet("{id:guid}/purchases")]
    public async Task<IActionResult> GetPurchases(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        => Ok(await Sender.Send(new GetClientPurchasesQuery(CurrentTenantId, id, page, pageSize), ct));

}
