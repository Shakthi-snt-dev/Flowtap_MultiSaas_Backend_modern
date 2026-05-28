using Flowtap_Application.Features.Subscription.Commands.StartTrial;
using Flowtap_Application.Features.Subscription.Queries.GetSubscription;
using Flowtap_Application.Features.Subscription.Queries.GetTrialStatus;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Presentation.Controllers;

[Route("api/v1/subscription")]
public class SubscriptionController(ISender sender) : ApiController(sender)
{
    [HttpPost("trial")]
    public async Task<IActionResult> StartTrial([FromBody] StartTrialCommand command, CancellationToken ct)
        => Created(await Sender.Send(command, ct));

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
        => Ok(await Sender.Send(new GetSubscriptionQuery(CurrentTenantId), ct));

    [HttpGet("trial")]
    public async Task<IActionResult> GetTrialStatus(CancellationToken ct)
        => Ok(await Sender.Send(new GetTrialStatusQuery(CurrentTenantId), ct));
}
