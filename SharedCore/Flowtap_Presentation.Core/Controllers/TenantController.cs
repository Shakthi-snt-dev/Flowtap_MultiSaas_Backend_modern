using Flowtap_Application.Features.Organization.Tenant.Commands.CreateTenant;
using Flowtap_Application.Features.Organization.Tenant.Commands.SeedIndustryData;
using Flowtap_Application.Features.Organization.Tenant.Commands.UpdateTenant;
using Flowtap_Application.Features.Organization.Tenant.Commands.UpdateTenantSettings;
using Flowtap_Application.Features.Organization.Tenant.Queries.GetTenant;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Presentation.Controllers;

[Route("api/v1/tenant")]
public class TenantController(ISender sender) : ApiController(sender)
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTenantCommand command, CancellationToken ct)
        => Created(await Sender.Send(command, ct));

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
        => Ok(await Sender.Send(new GetTenantQuery(CurrentTenantId), ct));

    [HttpPost("seed-industry")]
    public async Task<IActionResult> SeedIndustry([FromBody] SeedIndustryDataCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command, ct));

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateTenantCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { CompanyId = CurrentTenantId }, ct));

    [HttpPut("settings")]
    public async Task<IActionResult> UpdateSettings([FromBody] UpdateTenantSettingsCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { CompanyId = CurrentTenantId }, ct));
}
