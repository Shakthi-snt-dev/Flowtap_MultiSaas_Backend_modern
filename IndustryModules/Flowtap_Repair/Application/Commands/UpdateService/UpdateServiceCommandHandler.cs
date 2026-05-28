using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Repair.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Commands.UpdateService;

public class UpdateServiceCommandHandler(IRepairDbContext db)
    : IRequestHandler<UpdateServiceCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateServiceCommand request, CancellationToken ct)
    {
        var service = await db.Services
            .FirstOrDefaultAsync(s => s.CompanyId == request.CompanyId && s.Id == request.Id, ct)
            ?? throw new NotFoundException(nameof(Flowtap_Repair.Domain.Entities.Service), request.Id);

        if (request.Name is not null) service.Name = request.Name;
        if (request.Description is not null) service.Description = request.Description;
        if (request.BasePrice.HasValue) service.BasePrice = request.BasePrice.Value;
        if (request.EstimatedDuration is not null) service.EstimatedDuration = request.EstimatedDuration;
        if (request.IsActive.HasValue) service.IsActive = request.IsActive.Value;

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

