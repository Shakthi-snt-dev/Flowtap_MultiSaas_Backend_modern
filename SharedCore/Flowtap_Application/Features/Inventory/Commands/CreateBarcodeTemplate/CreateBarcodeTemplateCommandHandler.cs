using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.CreateBarcodeTemplate;

public class CreateBarcodeTemplateCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateBarcodeTemplateCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateBarcodeTemplateCommand request, CancellationToken ct)
    {
        // If marking as default, clear other defaults
        if (request.IsDefault)
        {
            var existingDefaults = await db.BarcodeTemplates
                .Where(t => t.CompanyId == request.CompanyId && t.IsDefault)
                .ToListAsync(ct);
            foreach (var t in existingDefaults)
                t.IsDefault = false;
        }

        var template = new BarcodeTemplate
        {
            CompanyId = request.CompanyId,
            Name = request.Name,
            CodeType = request.CodeType,
            IsDefault = request.IsDefault,
            LabelsPerRow = request.LabelsPerRow,
            LabelWidthMm = request.LabelWidthMm,
            LabelHeightMm = request.LabelHeightMm,
            ShowProductName = request.ShowProductName,
            ShowSKU = request.ShowSKU
        };

        db.BarcodeTemplates.Add(template);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(template.Id);
    }
}
