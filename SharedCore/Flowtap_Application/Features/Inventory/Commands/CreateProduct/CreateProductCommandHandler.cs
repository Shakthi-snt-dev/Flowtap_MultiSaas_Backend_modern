using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.CreateProduct;

public class CreateProductCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken ct)
    {
        if (!Enum.TryParse<ProductKind>(request.Kind, true, out var kind))
            return Result<Guid>.Failure($"Invalid product kind: {request.Kind}");

        // Auto-generate SKU if not provided
        string sku = request.SKU ?? await GenerateSkuAsync(request.CompanyId, ct);

        var product = new Product
        {
            CompanyId = request.CompanyId,
            CategoryId = request.CategoryId,
            Name = request.Name,
            Kind = kind,
            SKU = sku,
            DefaultCostPrice = request.DefaultCostPrice,
            DefaultSalePrice = request.DefaultSalePrice,
            IsSerialized = request.IsSerialized,
            IsUniversal = request.IsUniversal,
            TaxSlabId = request.TaxSlabId,
            HsnCode = request.HsnCode,
            PublishStatus = ProductPublishStatus.Draft
        };

        db.Products.Add(product);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(product.Id);
    }

    private async Task<string> GenerateSkuAsync(Guid companyId, CancellationToken ct)
    {
        var count = await db.Products.CountAsync(p => p.CompanyId == companyId, ct);
        return $"SKU-{count + 1:D6}";
    }
}
