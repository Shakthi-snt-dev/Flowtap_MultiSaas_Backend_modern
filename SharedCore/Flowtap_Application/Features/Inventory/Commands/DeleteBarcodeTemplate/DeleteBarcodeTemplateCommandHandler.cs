using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.DeleteBarcodeTemplate;

public class DeleteBarcodeTemplateCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteBarcodeTemplateCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteBarcodeTemplateCommand request, CancellationToken ct)
    {
        var template = await db.BarcodeTemplates
            .FirstOrDefaultAsync(t => t.Id == request.Id && t.CompanyId == request.CompanyId, ct);
        if (template is null)
            return Result<bool>.Failure("Barcode template not found.");

        db.BarcodeTemplates.Remove(template);
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
