using Flowtap_Application.Features.Inventory.Commands.ArchiveProduct;
using Flowtap_Application.Features.Inventory.Commands.CreateProduct;
using Flowtap_Application.Features.Inventory.Commands.UpdateProduct;
using Flowtap_Application.Features.Inventory.Commands.CreateProductVariant;
using Flowtap_Application.Features.Inventory.Commands.UpdateProductVariant;
using Flowtap_Application.Features.Inventory.Commands.DeleteProductVariant;
using Flowtap_Application.Features.Inventory.Commands.SetProductLocationPrice;
using Flowtap_Application.Features.Inventory.Commands.AddProductMedia;
using Flowtap_Application.Features.Inventory.Commands.DeleteProductMedia;
using Flowtap_Application.Features.Inventory.Queries.GetProduct;
using Flowtap_Application.Features.Inventory.Queries.GetProducts;
using Flowtap_Application.Features.Inventory.Queries.GetProductVariants;
using Flowtap_Application.Features.Inventory.Queries.GetProductLocationPrices;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Presentation.Controllers;

[Flowtap_Presentation.Authorization.RequirePermission("Inventory")]
[Route("api/v1/products")]
public class ProductController(ISender sender) : ApiController(sender)
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command, CancellationToken ct)
        => Created(await Sender.Send(command, ct));

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetProductsQuery query, CancellationToken ct)
        => Ok(await Sender.Send(query, ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await Sender.Send(new GetProductQuery(CurrentTenantId, id), ct));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { Id = id, CompanyId = CurrentTenantId }, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Archive(Guid id, CancellationToken ct)
        => FromResult(await Sender.Send(new ArchiveProductCommand(CurrentTenantId, id), ct));

    // ── Variants ───────────────────────────────────────────────────────────────

    [HttpGet("{productId:guid}/variants")]
    public async Task<IActionResult> GetVariants(Guid productId, CancellationToken ct)
        => Ok(await Sender.Send(new GetProductVariantsQuery(productId), ct));

    [HttpPost("{productId:guid}/variants")]
    public async Task<IActionResult> CreateVariant(Guid productId, [FromBody] CreateProductVariantCommand command, CancellationToken ct)
        => Created(await Sender.Send(command with { ProductId = productId }, ct));

    [HttpPut("{productId:guid}/variants/{id:guid}")]
    public async Task<IActionResult> UpdateVariant(Guid productId, Guid id, [FromBody] UpdateProductVariantCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command with { Id = id, ProductId = productId }, ct));

    [HttpDelete("{productId:guid}/variants/{id:guid}")]
    public async Task<IActionResult> DeleteVariant(Guid productId, Guid id, CancellationToken ct)
        => FromResult(await Sender.Send(new DeleteProductVariantCommand(id, productId), ct));

    // ── Location Prices ────────────────────────────────────────────────────────

    [HttpGet("{productId:guid}/prices")]
    public async Task<IActionResult> GetPrices(Guid productId, CancellationToken ct)
        => Ok(await Sender.Send(new GetProductLocationPricesQuery(CurrentTenantId, productId), ct));

    [HttpPost("{productId:guid}/prices")]
    public async Task<IActionResult> SetPrice(Guid productId, [FromBody] SetProductLocationPriceCommand command, CancellationToken ct)
        => Ok(await Sender.Send(command with { CompanyId = CurrentTenantId, ProductId = productId }, ct));

    // ── Media ──────────────────────────────────────────────────────────────────

    [HttpPost("{productId:guid}/media")]
    public async Task<IActionResult> AddMedia(Guid productId, [FromBody] AddProductMediaCommand command, CancellationToken ct)
        => Created(await Sender.Send(command with { ProductId = productId }, ct));

    [HttpDelete("{productId:guid}/media/{mediaId:guid}")]
    public async Task<IActionResult> DeleteMedia(Guid productId, Guid mediaId, CancellationToken ct)
        => FromResult(await Sender.Send(new DeleteProductMediaCommand(mediaId, productId), ct));

}
