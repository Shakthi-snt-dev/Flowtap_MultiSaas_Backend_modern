using Flowtap_Repair.Application.Commands.CreateService;
using Flowtap_Repair.Application.Commands.UpdateService;
using Flowtap_Repair.Application.Queries.GetService;
using Flowtap_Repair.Application.Queries.GetServices;
using Flowtap_Presentation.Authorization;
using Flowtap_Presentation.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Repair.Controllers;

[RequirePermission("ServiceTickets")]
[Route("api/v1/services")]
public class ServiceController(ISender sender) : ApiController(sender)
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateServiceCommand command, CancellationToken ct)
        => Created(await Sender.Send(command, ct));

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetServicesQuery query, CancellationToken ct)
        => Ok(await Sender.Send(query, ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await Sender.Send(new GetServiceQuery(CurrentTenantId, id), ct));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateServiceCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { Id = id, CompanyId = CurrentTenantId }, ct));
}
