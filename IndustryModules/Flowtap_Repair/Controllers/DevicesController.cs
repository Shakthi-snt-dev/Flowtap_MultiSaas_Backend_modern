using Flowtap_Repair.Application.Features.Devices.Commands.CreateDeviceBrand;
using Flowtap_Repair.Application.Features.Devices.Commands.UpdateDeviceBrand;
using Flowtap_Repair.Application.Features.Devices.Commands.DeleteDeviceBrand;
using Flowtap_Repair.Application.Features.Devices.Commands.CreateDeviceModel;
using Flowtap_Repair.Application.Features.Devices.Commands.UpdateDeviceModel;
using Flowtap_Repair.Application.Features.Devices.Commands.DeleteDeviceModel;
using Flowtap_Repair.Application.Features.Devices.Queries.GetDeviceBrands;
using Flowtap_Repair.Application.Features.Devices.Queries.GetDeviceModels;
using Flowtap_Repair.Application.Features.Devices.Queries.GetProductsByDeviceModel;
using Flowtap_Repair.Application.Queries.GetServicesByModel;
using Flowtap_Presentation.Authorization;
using Flowtap_Presentation.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Repair.Controllers;

[RequirePermission("Inventory")]
[Route("api/v1/devices")]
public class DevicesController(ISender sender) : ApiController(sender)
{
    [HttpPost("brands")]
    public async Task<IActionResult> CreateBrand([FromBody] CreateDeviceBrandCommand command, CancellationToken ct)
        => Created(await Sender.Send(command, ct));

    [HttpGet("brands")]
    public async Task<IActionResult> GetBrands([FromQuery] GetDeviceBrandsQuery query, CancellationToken ct)
        => Ok(await Sender.Send(query, ct));

    [HttpPut("brands/{id:guid}")]
    public async Task<IActionResult> UpdateBrand(Guid id, [FromBody] UpdateDeviceBrandCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { Id = id, CompanyId = CurrentTenantId }, ct));

    [HttpDelete("brands/{id:guid}")]
    public async Task<IActionResult> DeleteBrand(Guid id, CancellationToken ct)
        => FromResult(await Sender.Send(new DeleteDeviceBrandCommand(id, CurrentTenantId), ct));

    [HttpPost("models")]
    public async Task<IActionResult> CreateModel([FromBody] CreateDeviceModelCommand command, CancellationToken ct)
        => Created(await Sender.Send(command, ct));

    [HttpGet("models")]
    public async Task<IActionResult> GetModels([FromQuery] GetDeviceModelsQuery query, CancellationToken ct)
        => Ok(await Sender.Send(query, ct));

    [HttpPut("models/{id:guid}")]
    public async Task<IActionResult> UpdateModel(Guid id, [FromBody] UpdateDeviceModelCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { Id = id, CompanyId = CurrentTenantId }, ct));

    [HttpDelete("models/{id:guid}")]
    public async Task<IActionResult> DeleteModel(Guid id, CancellationToken ct)
        => FromResult(await Sender.Send(new DeleteDeviceModelCommand(id, CurrentTenantId), ct));

    [HttpGet("models/{modelId}/products")]
    public async Task<IActionResult> GetProductsByModel(Guid modelId, [FromQuery] Guid companyId, CancellationToken ct)
        => Ok(await Sender.Send(new GetProductsByDeviceModelQuery(companyId, modelId), ct));

    [HttpGet("models/{modelId}/services")]
    public async Task<IActionResult> GetServicesByModel(Guid modelId, [FromQuery] Guid companyId, CancellationToken ct)
        => Ok(await Sender.Send(new GetServicesByModelQuery(modelId, companyId), ct));
}
