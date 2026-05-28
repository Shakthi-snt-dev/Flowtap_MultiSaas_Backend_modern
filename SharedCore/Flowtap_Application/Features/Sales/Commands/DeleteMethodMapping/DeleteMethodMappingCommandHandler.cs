using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Sales.Commands.DeleteMethodMapping;

public class DeleteMethodMappingCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteMethodMappingCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteMethodMappingCommand request, CancellationToken ct)
    {
        var mapping = await db.PaymentMethodMappings
            .FirstOrDefaultAsync(m => m.Id == request.Id && m.CompanyId == request.CompanyId, ct)
            ?? throw new NotFoundException("PaymentMethodMapping", request.Id);

        db.PaymentMethodMappings.Remove(mapping);
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
