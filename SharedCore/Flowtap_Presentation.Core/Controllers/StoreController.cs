using Flowtap_Application.Features.Organization.Store.Commands.CreateStore;
using Flowtap_Application.Features.Organization.Store.Commands.UpdateStore;
using Flowtap_Application.Features.Organization.Store.Commands.DeleteStore;
using Flowtap_Application.Features.Organization.Store.Commands.SetDefaultStore;
using Flowtap_Application.Features.Organization.Store.Queries.GetStore;
using Flowtap_Application.Features.Organization.Store.Queries.GetStores;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Presentation.Controllers;

[Route("api/v1/stores")]
public class StoreController(ISender sender) : ApiController(sender)
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStoreCommand command, CancellationToken ct)
        => Created(await Sender.Send(command, ct));

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetStoresQuery query, CancellationToken ct)
        => Ok(await Sender.Send(query, ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await Sender.Send(new GetStoreQuery(CurrentTenantId, id), ct));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStoreCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { Id = id, CompanyId = CurrentTenantId }, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => Ok(await Sender.Send(new DeleteStoreCommand(id), ct));

    [HttpPost("default")]
    public async Task<IActionResult> SetDefault([FromBody] SetDefaultStoreRequest req, CancellationToken ct)
        => Ok(await Sender.Send(new SetDefaultStoreCommand(req.StoreId), ct));
}

public record SetDefaultStoreRequest(Guid StoreId);
