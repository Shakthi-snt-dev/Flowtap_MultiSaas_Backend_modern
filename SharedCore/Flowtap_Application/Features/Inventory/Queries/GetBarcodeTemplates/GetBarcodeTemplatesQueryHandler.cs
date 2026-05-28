using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Queries.GetBarcodeTemplates;

public class GetBarcodeTemplatesQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetBarcodeTemplatesQuery, Result<List<BarcodeTemplateDto>>>
{
    public async Task<Result<List<BarcodeTemplateDto>>> Handle(GetBarcodeTemplatesQuery request, CancellationToken ct)
    {
        var items = await db.BarcodeTemplates
            .Where(t => t.CompanyId == request.CompanyId)
            .OrderByDescending(t => t.IsDefault).ThenBy(t => t.Name)
            .Select(t => new BarcodeTemplateDto(
                t.Id, t.Name, t.CodeType, t.IsDefault, t.LabelsPerRow,
                t.LabelWidthMm, t.LabelHeightMm, t.ShowProductName, t.ShowSKU))
            .ToListAsync(ct);

        return Result<List<BarcodeTemplateDto>>.Success(items);
    }
}
