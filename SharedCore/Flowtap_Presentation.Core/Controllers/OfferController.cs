using Flowtap_Application.Features.Sales.Commands.CreateOffer;
using Flowtap_Application.Features.Sales.Commands.DeleteOffer;
using Flowtap_Application.Features.Sales.Commands.UpdateOffer;
using Flowtap_Application.Features.Sales.Queries.GetOffers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Presentation.Controllers;

[Route("api/v1/offers")]
public class OfferController(ISender sender) : ApiController(sender)
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid companyId, [FromQuery] bool activeOnly = false, CancellationToken ct = default)
        => Ok(await Sender.Send(new GetOffersQuery(companyId == Guid.Empty ? CurrentTenantId : companyId, activeOnly), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOfferCommand command, CancellationToken ct)
        => Created(await Sender.Send(command with { CompanyId = CurrentTenantId }, ct));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOfferCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { Id = id, CompanyId = CurrentTenantId }, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => FromResult(await Sender.Send(new DeleteOfferCommand(id, CurrentTenantId), ct));
}
