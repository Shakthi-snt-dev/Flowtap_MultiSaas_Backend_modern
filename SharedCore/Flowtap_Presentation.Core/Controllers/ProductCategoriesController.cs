using Flowtap_Application.Features.Inventory.Commands.CreateProductCategory;
using Flowtap_Application.Features.Inventory.Commands.UpdateProductCategory;
using Flowtap_Application.Features.Inventory.Commands.DeleteProductCategory;
using Flowtap_Application.Features.Inventory.Queries.GetProductCategories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Presentation.Controllers;

[Flowtap_Presentation.Authorization.RequirePermission("Inventory")]
[Route("api/v1/product-categories")]
public class ProductCategoriesController(ISender sender) : ApiController(sender)
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCategoryCommand command, CancellationToken ct)
        => Created(await Sender.Send(command, ct));

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetProductCategoriesQuery query, CancellationToken ct)
        => Ok(await Sender.Send(query, ct));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCategoryCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { Id = id, CompanyId = CurrentTenantId }, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => FromResult(await Sender.Send(new DeleteProductCategoryCommand(id, CurrentTenantId), ct));
}
