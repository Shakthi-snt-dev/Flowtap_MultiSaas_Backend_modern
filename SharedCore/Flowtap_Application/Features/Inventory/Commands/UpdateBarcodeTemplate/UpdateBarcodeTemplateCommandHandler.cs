using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.UpdateBarcodeTemplate;

public class UpdateBarcodeTemplateCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateBarcodeTemplateCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateBarcodeTemplateCommand request, CancellationToken ct)
    {
        var template = await db.BarcodeTemplates
            .FirstOrDefaultAsync(t => t.Id == request.Id && t.CompanyId == request.CompanyId, ct);
        if (template is null)
            return Result<bool>.Failure("Barcode template not found.");

        // If marking as default, clear other defaults first
        if (request.IsDefault == true)
        {
            var others = await db.BarcodeTemplates
                .Where(t => t.CompanyId == request.CompanyId && t.IsDefault && t.Id != request.Id)
                .ToListAsync(ct);
            foreach (var t in others) t.IsDefault = false;
        }

        if (request.Name is not null)               template.Name = request.Name;
        if (request.CodeType is not null)           template.CodeType = request.CodeType;
        if (request.IsDefault.HasValue)             template.IsDefault = request.IsDefault.Value;
        if (request.LabelsPerRow.HasValue)          template.LabelsPerRow = request.LabelsPerRow.Value;
        if (request.LabelWidthMm.HasValue)          template.LabelWidthMm = request.LabelWidthMm.Value;
        if (request.LabelHeightMm.HasValue)         template.LabelHeightMm = request.LabelHeightMm.Value;
        if (request.ShowProductName.HasValue)       template.ShowProductName = request.ShowProductName.Value;
        if (request.ShowSKU.HasValue)               template.ShowSKU = request.ShowSKU.Value;

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
